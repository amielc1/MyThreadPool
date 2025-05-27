using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyThreadPool.Services;
using MyThreadPool.Settings;
using System.Threading.Tasks;

public class ThreadPoolTests
{
    [Fact]
    public async Task Execute_ShouldInvokeTask()
    {
        var logger = TestLoggerFactory.CreateLogger<ThreadPoolImp>();

        var optionsMock = new Mock<IOptions<ThreadPoolSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new ThreadPoolSettings { MaxThreads = 3 });

        var threadPool = new ThreadPoolImp(optionsMock.Object, logger);

        var taskCompleted = new ManualResetEvent(false);
        logger.LogInformation("TEST START");
        for (int i = 0; i < 6; i++)
        {
            ThreadStart task = () =>
                   {
                       logger.LogDebug($" - Create thread {i} sleep for {1000*i}");
                       Thread.Sleep(1000 * i);
                       logger.LogDebug($" - thread {i} wakeup");
                       taskCompleted.Set();
                   };

            // Act
            threadPool.Execute(task);
        }
       

        // Wait for task execution (max 2 seconds to avoid test hang)
        var signaled = taskCompleted.WaitOne(TimeSpan.FromSeconds(2)); 
        logger.LogInformation("TEST END");


        while (true)
        {
            await Task.Delay(1000);
        }
    }
}
