using System.Collections.Concurrent;
using System.Diagnostics;
using DevBase.Generics;

namespace DevBase.Async.Task;

using Task = System.Threading.Tasks.Task;

public class Multitasking : IDisposable
{
    private AList<(Task, CancellationTokenSource)> _activeTasks;
    private ConcurrentQueue<(Task, CancellationTokenSource)> _parkedTasks;

    private CancellationTokenSource _cancellationTokenSource;

    private readonly int _capacity;
    private readonly int _scheduleDelay;

    private bool _disposed;
    
    public Multitasking(int capacity, int scheduleDelay = 100)
    {
        this._parkedTasks = new ConcurrentQueue<(Task, CancellationTokenSource)>();
        this._activeTasks = new AList<(Task, CancellationTokenSource)>();

        this._capacity = capacity;
        this._scheduleDelay = scheduleDelay;
        this._disposed = false;

        this._cancellationTokenSource = new CancellationTokenSource();
        
        Task.Factory.StartNew(HandleTasks);
    }

    private async Task HandleTasks()
    {
        while (!this._disposed)
        {
            await Task.Delay(this._scheduleDelay);
            
            CheckAndRemove();
            
            if (this._activeTasks.Length >= this._capacity)
                continue;
            
            ActivateTask();
        }
    }

    public async Task WaitAll()
    {
        while (!this._disposed && 
               !(this._parkedTasks.Count == 0 && this._activeTasks.Length == 0))
        {
            await Task.Delay(this._scheduleDelay);
            
            if (this._parkedTasks.Count != 0 && this._activeTasks.Length == 0)
                continue;
            
            for (int i = 0; i < this._activeTasks.Length; i++)
            {
                (Task, CancellationTokenSource) taskToken = this._activeTasks.Get(i);
                await taskToken.Item1.WaitAsync(taskToken.Item2.Token);
            }
        }
    }

    public async Task KillAll()
    {
        this._activeTasks.ForEach(t=>t.Item2.Cancel());
        
        foreach (var parkedTask in this._parkedTasks)
        {
            parkedTask.Item2.Cancel();
        }

        await WaitAll();
    }

    private void ActivateTask()
    {
        (Task, CancellationTokenSource) task;
        this._parkedTasks.TryDequeue(out task);

        if (task.Item1 == null || task.Item2 == null)
            return;
        
        task.Item1.Start();
    }
    
    private void CheckAndRemove()
    {
        for (int i = 0; i < this._activeTasks.Length; i++)
        {
            (Task, CancellationTokenSource) currentTask = this._activeTasks.Get(i);

            if (currentTask.Item1.IsCanceled || 
                currentTask.Item1.IsCompleted || 
                currentTask.Item1.IsCompletedSuccessfully)
            {
                this._activeTasks.SafeRemove(currentTask);
            }
        }
    }
    
    public Task Register(Task task)
    {
        this._parkedTasks.Enqueue((task, this._cancellationTokenSource));
        return task;
    }

    public Task Register(Action action) => Register(new Task(action));

    public void Dispose()
    {
        this._disposed = true;

        KillAll();
    }
}