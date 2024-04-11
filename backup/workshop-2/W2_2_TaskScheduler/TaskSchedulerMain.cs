using System.Collections.Concurrent;

namespace W2_2_TaskScheduler;

public class TaskSchedulerMain
{
    public static async Task Main(string[] args)
    {
        using var scheduler = new ThrottlerTaskScheduler(2);
        
        var tasks = new List<Task>();
        
        Console.WriteLine($"begin Main: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
        
        for (int i = 0; i < 10; i++)
        {
            var a = i;
            
            var task = Task.Factory.StartNew(async () =>
            {
                Console.WriteLine($"begin task1: {a}, threadId: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                Console.WriteLine($"end task1: {a}, threadId: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
                
                Console.WriteLine($"begin task2: {a}, threadId: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                Console.WriteLine($"end task2: {a}, threadId: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
                
                Console.WriteLine($"begin task3: {a}, threadId: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                Console.WriteLine($"end task3: {a}, threadId: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
            }, CancellationToken.None, TaskCreationOptions.None, scheduler).Unwrap();
            
            tasks.Add(task);
        }

        Console.WriteLine($"before Main await: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
        await Task.WhenAll(tasks);
        Console.WriteLine($"after Main await: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
        
        Console.WriteLine($"begin pre-latest: threadId: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
        await Task.Delay(TimeSpan.FromSeconds(2));
        Console.WriteLine($"end pre-latest: threadId: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
                
        Console.WriteLine($"begin latest: threadId: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
        await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
        Console.WriteLine($"end latest: threadId: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
        
        await Task.Delay(TimeSpan.FromSeconds(2));

        var text = await File.ReadAllTextAsync(@"C:\Users\vpunin\Desktop\C#\2-3\Workshop2\Workshop2.sln");
        Console.WriteLine($"end latest: threadId: {Environment.CurrentManagedThreadId}, {Thread.CurrentThread.Name}, {SynchronizationContext.Current}");
    }
}

public class ThrottlerTaskScheduler : TaskScheduler, IDisposable
{
    private readonly ConcurrentQueue<Task> _tasksQueue = new ConcurrentQueue<Task>();
    private readonly ManualResetEvent _disposeEvent = new ManualResetEvent(false);
    private readonly Semaphore _queueSemaphore;
    private readonly CountdownEvent _disposeCompletedEvent;
    private readonly Thread[] _threads;

    public ThrottlerTaskScheduler(int workersCount)
    {
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
