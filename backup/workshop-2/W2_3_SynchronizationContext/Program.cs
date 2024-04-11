using System.Collections.Concurrent;

namespace W2_3_SynchronizationContext;

public class TaskSchedulerMain
{
    public static async Task Main(string[] args)
    {
        using var scheduler = new ThrottlerTaskScheduler(2);

        Log("begin main");
        
        SynchronizationContext.SetSynchronizationContext(scheduler.SynchronizationContext);

        Log("begin first delay");
        await Task.Delay(TimeSpan.FromSeconds(2));
        Log("end first delay");
        
        var tasks = new List<Task>();

        for (int i = 0; i < 2; i++)
        {
            var a = i;
            
            var task = Task.Factory.StartNew(async () =>
            {
                Log($"begin task1: {a}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                Log($"end task1: {a}");
                
                Log($"begin task2: {a}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                Log($"end task2: {a}");
                
                Log($"begin task3: {a}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                Log($"end task3: {a}");
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Unwrap();
            
            tasks.Add(task);
        }

        Log($"before all tasks await");
        await Task.WhenAll(tasks);
        Log($"after all tasks await");
        
        Log("begin pre-latest");
        await Task.Delay(TimeSpan.FromSeconds(2));
        Log("end pre-latest");

        Log("begin latest");
        await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
        Log("end latest");
    }

    private static void Log(string prefix)
    {
        Console.WriteLine($"{prefix}: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");   
    }
}

public class ThrottlerSynchronizationContext : SynchronizationContext
{
    private readonly ThrottlerTaskScheduler _scheduler;

    public ThrottlerSynchronizationContext(ThrottlerTaskScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        _scheduler.Run(() => d(state));
    }
}

public class ThrottlerTaskScheduler : TaskScheduler, IDisposable
{
    private readonly ConcurrentQueue<Task> _tasksQueue = new ConcurrentQueue<Task>();
    private readonly ManualResetEvent _disposeEvent = new ManualResetEvent(false);
    private readonly Semaphore _queueSemaphore;
    private readonly CountdownEvent _disposeCompletedEvent;
    private readonly Thread[] _threads;
    
    public SynchronizationContext SynchronizationContext { get; }
    
    public ThrottlerTaskScheduler(int workersCount)
    {
        SynchronizationContext = new ThrottlerSynchronizationContext(this);
        
        _disposeCompletedEvent = new CountdownEvent(workersCount);

        _queueSemaphore = new Semaphore(0, Int32.MaxValue);
        
        _threads = new Thread[workersCount];
        for (int i = 0; i < workersCount; i++)
        {
            var thread = new Thread(TryExecuteTasks);
            thread.Name = $"[{nameof(ThrottlerTaskScheduler)}-{i}]";
            thread.Start();

            _threads[i] = thread;
        }
    }

    public Task Run(Action action)
    {
        return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this);
    }

    private void TryExecuteTasks()
    {
        SynchronizationContext.SetSynchronizationContext(SynchronizationContext);
        
        while (true)
        {
            var result = WaitHandle.WaitAny(new WaitHandle[]{_queueSemaphore, _disposeEvent});
            
            if (_tasksQueue.TryDequeue(out var task))
            {
                TryExecuteTask(task);
                continue;
            }
            
            break;
        }
        
        _disposeCompletedEvent.Signal();
    }

    protected override IEnumerable<Task>? GetScheduledTasks()
    {
        return _tasksQueue.ToArray();
    }

    protected override void QueueTask(Task task)
    {
        _tasksQueue.Enqueue(task);
        _queueSemaphore.Release();
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        return false;
    }

    public void Dispose()
    {
        _disposeEvent.Set();
        _disposeCompletedEvent.Wait();

        foreach (var thread in _threads)
        {
            thread.Join();
        }

        _disposeEvent.Dispose();
        _disposeCompletedEvent.Dispose();
        
        _queueSemaphore.Dispose();
    }
}
