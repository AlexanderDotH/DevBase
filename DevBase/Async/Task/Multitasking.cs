using System.Collections.Concurrent;
using System.Diagnostics;
using DevBase.Generics;

namespace DevBase.Async.Task;

using Task = System.Threading.Tasks.Task;

public class Multitasking
{
    private ConcurrentQueue<(Task, CancellationTokenSource)> _parkedTasks;
    private ConcurrentDictionary<Task, CancellationTokenSource> _activeTasks;

    private CancellationTokenSource _cancellationTokenSource;

    private readonly int _capacity;
    private readonly int _scheduleDelay;

    private bool _disposed;
    
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

    public async Task WaitAll()
    {
        while (!_disposed && _parkedTasks.Count > 0)
        {
            await Task.Delay(_scheduleDelay);
        }

        var activeTasks = _activeTasks.Keys.ToArray();
        await Task.WhenAll(activeTasks);
    }

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
    
    public Task Register(Task task)
    {
        this._parkedTasks.Enqueue((task, this._cancellationTokenSource));
        return task;
    }

    public Task Register(Action action) => Register(new Task(action));
}