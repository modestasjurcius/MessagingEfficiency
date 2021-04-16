using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResultsMicroservice.Entities;
using ResultsMicroservice.Services;

namespace ResultsMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly ILogger<ResultsController> _logger;
        private readonly IResultsService _service;

        public ResultsController(ILogger<ResultsController> logger, IResultsService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [Route("[action]")]
        public ServiceResponse Get()
        {
            return new ServiceResponse() { Success = true, Message = "Results microservice is alive" };
        }

        [HttpPost]
        [Route("[action]")]
        public ServiceResponse Insert(RabbitTestResult result)
        {
            return _service.InsertRabbitResult(result);
        }
    }
}
