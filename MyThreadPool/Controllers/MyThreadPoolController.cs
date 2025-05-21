using Microsoft.AspNetCore.Mvc;

namespace MyThreadPool.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyThreadPoolController : ControllerBase
    {
          
        private readonly ILogger<MyThreadPoolController> _logger;

        public MyThreadPoolController(ILogger<MyThreadPoolController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetThread")]
        public string Get()
        {
            return "MyThreadPoolController -> GetThread";
        }
    }
}
