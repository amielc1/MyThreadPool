using Microsoft.Extensions.Options;
using MyThreadPool.Settings;
using System.Collections.Concurrent;

namespace MyThreadPool.Services;
public class ThreadPoolImp : IThreadPool
{
    private readonly ConcurrentQueue<ThreadStart> _taskQueue = new();
    private readonly SemaphoreSlim _taskAvailable;
    private readonly List<Thread> _workers = new();
    private readonly CancellationTokenSource cts = new();
    private readonly ILogger<ThreadPoolImp> _logger;
    private  int _maxThreads;
    private bool isDisposed = false;

    public ThreadPoolImp(IOptions<ThreadPoolSettings> options, ILogger<ThreadPoolImp> logger)
    { 
        _maxThreads = options.Value.MaxThreads;
        if (_maxThreads <= 0) throw new ArgumentException("Thread count must be positive.", nameof(_maxThreads));
        _logger = logger;
        _taskAvailable = new SemaphoreSlim(0);

        logger.LogInformation("Initializing ThreadPool with {ThreadCount} threads", _maxThreads);

        for (int i = 0; i < _maxThreads; i++)
        {
            var thread = new Thread(() => WorkerLoop(cts.Token))
            {
                IsBackground = true,
                Name = $"Worker-{i}"
            };
            thread.Start();
            _workers.Add(thread);
        }
    }


    public void Execute(ThreadStart task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        if (isDisposed) throw new ObjectDisposedException(nameof(MyThreadPool));

        _taskQueue.Enqueue(task);
        _taskAvailable.Release();
        _logger.LogDebug("Task enqueued. Total queued: {Count}", _taskQueue.Count);
    }

    private void WorkerLoop(CancellationToken token)
    {
        while (true)
        {
            try
            {
                _taskAvailable.Wait(token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Worker thread {ThreadName} stopping due to cancellation", Thread.CurrentThread.Name);
                break;
            }

            if (_taskQueue.TryDequeue(out var task))
            {
                try
                {
                    _logger.LogDebug("Worker {ThreadName} executing task", Thread.CurrentThread.Name);
                    task.Invoke();
                    _logger.LogDebug("Worker {ThreadName} finished task", Thread.CurrentThread.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception during task execution in thread {ThreadName}", Thread.CurrentThread.Name);
                }
            }
        }
    }

    public void Dispose()
    {
        if (isDisposed) return;

        isDisposed = true;
        cts.Cancel();

        _logger.LogInformation("Disposing ThreadPool and signaling all worker threads to stop");

        for (int i = 0; i < _maxThreads; i++)
        {
            _taskAvailable.Release(); // wake workers
        }

        foreach (var thread in _workers)
        {
            thread.Join();
            _logger.LogInformation("Thread {ThreadName} exited", thread.Name);
        }

        _taskAvailable.Dispose();
        cts.Dispose();

        _logger.LogInformation("ThreadPool disposed");
    }
}
