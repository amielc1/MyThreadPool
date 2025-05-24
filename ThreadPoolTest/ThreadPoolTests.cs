using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyThreadPool.Services;
using MyThreadPool.Settings;
using Xunit;

public class ThreadPoolTests
{
    [Fact]
    public void Execute_ShouldInvokeTask()
    {
        // Arrange
        var logger = new LoggerFactory().CreateLogger<ThreadPoolImp>();

        var optionsMock = new Mock<IOptions<ThreadPoolSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new ThreadPoolSettings { MaxThreads = 1 });

        var threadPool = new ThreadPoolImp(optionsMock.Object, logger);

        var wasCalled = false;
        var taskCompleted = new ManualResetEvent(false);

        ThreadStart task = () =>
        {
            wasCalled = true;
            taskCompleted.Set(); // Signal the test that the task has finished
        };

        // Act
        threadPool.Execute(task);

        // Wait for task execution (max 2 seconds to avoid test hang)
        var signaled = taskCompleted.WaitOne(TimeSpan.FromSeconds(2));

        // Cleanup
        threadPool.Dispose();

        // Assert
        Assert.True(signaled, "Task was not executed within the expected time.");
        Assert.True(wasCalled, "Task was not executed.");
    }
}
