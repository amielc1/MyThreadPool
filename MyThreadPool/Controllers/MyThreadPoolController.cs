using Microsoft.AspNetCore.Mvc;
using MyThreadPool.Services;

namespace MyThreadPool.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyThreadPoolController : ControllerBase
    {
          
        private readonly ILogger<MyThreadPoolController> _logger;
        private readonly IThreadPool _threadPool;

        public MyThreadPoolController(ILogger<MyThreadPoolController> logger, IThreadPool threadPool)
        {
            _logger = logger;
            _threadPool = threadPool;   
        }

        [HttpGet(Name = "GetThread")]
        public string Get()
        {
            _logger.LogInformation("GetThread called"); 
            _threadPool.Execute(() =>
            {
                var sleepTime =10000;
                _logger.LogInformation("START Executing task on thread {ThreadId} , Sleep for {sleepTime} sec", Thread.CurrentThread.ManagedThreadId,sleepTime);
                Thread.Sleep(sleepTime);
                _logger.LogInformation("STOP Task completed on thread {ThreadId}", Thread.CurrentThread.ManagedThreadId);

               
            });
            _logger.LogInformation("Task enqueued successfully");
            return "Task enqueued successfully";
        }
    }
}
