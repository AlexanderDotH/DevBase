using System.Collections.Concurrent;
using System.Diagnostics;
using DevBase.Generics;

namespace DevBase.Async.Task;

using Task = System.Threading.Tasks.Task;

/// <summary>
/// Manages asynchronous tasks execution with capacity limits and scheduling.
/// </summary>
public class Multitasking
{
    private ConcurrentQueue<(Task, CancellationTokenSource)> _parkedTasks;
    private ConcurrentDictionary<Task, CancellationTokenSource> _activeTasks;

    private CancellationTokenSource _cancellationTokenSource;

    private readonly int _capacity;
    private readonly int _scheduleDelay;

    private bool _disposed;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Multitasking"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of concurrent tasks.</param>
    /// <param name="scheduleDelay">The delay between schedule checks in milliseconds.</param>
    public Multitasking(int capacity, int scheduleDelay = 100)
    {
        this._parkedTasks = new ConcurrentQueue<(Task, CancellationTokenSource)>();
        this._activeTasks = new ConcurrentDictionary<Task, CancellationTokenSource>();

        this._capacity = capacity;
        this._scheduleDelay = scheduleDelay;
        this._disposed = false;

        this._cancellationTokenSource = new CancellationTokenSource();
        
        Task.Factory.StartNew(HandleTasks);
    }

    private async Task HandleTasks()
    {
        while (!_disposed)
        {
            await Task.Delay(_scheduleDelay);

            CheckAndRemove();

            while (_activeTasks.Count < _capacity && _parkedTasks.TryDequeue(out var task))
            {
                _activeTasks.TryAdd(task.Item1, task.Item2);
                Task.Run(() => task.Item1.Start());
            }
        }
    }

    /// <summary>
    /// Waits for all scheduled tasks to complete.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task WaitAll()
    {
        while (!_disposed && _parkedTasks.Count > 0)
        {
            await Task.Delay(_scheduleDelay);
        }

        var activeTasks = _activeTasks.Keys.ToArray();
        await Task.WhenAll(activeTasks);
    }

    /// <summary>
    /// Cancels all tasks and waits for them to complete.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task KillAll()
    {
        foreach (var parkedTask in this._parkedTasks)
        {
            parkedTask.Item2.Cancel();
        }

        await WaitAll();
    }
    
    private void CheckAndRemove()
    {
        List<Task> tasksToRemove = new List<Task>();

        foreach (var kvp in _activeTasks)
        {
            var task = kvp.Key;
            if (task.IsCompleted || task.IsCanceled || task.IsFaulted)
            {
                tasksToRemove.Add(task);
            }
        }

        foreach (var task in tasksToRemove)
        {
            _activeTasks.TryRemove(task, out _);
        }
    }
    
    /// <summary>
    /// Registers a task to be managed.
    /// </summary>
    /// <param name="task">The task to register.</param>
    /// <returns>The registered task.</returns>
    public Task Register(Task task)
    {
        this._parkedTasks.Enqueue((task, this._cancellationTokenSource));
        return task;
    }

    /// <summary>
    /// Registers an action as a task to be managed.
    /// </summary>
    /// <param name="action">The action to register.</param>
    /// <returns>The task created from the action.</returns>
    public Task Register(Action action) => Register(new Task(action));
}