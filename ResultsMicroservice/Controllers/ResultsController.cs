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
        public ServiceResponse InsertRabbit(TestResult result)
        {
            return _service.InsertRabbitResult(result);
        }

        [HttpPost]
        [Route("[action]")]
        public ServiceResponse UpdateRabbitLastReceived(TestLastReceived args)
        {
            return _service.UpdateRabbitResult(args);
        }

        [HttpPost]
        [Route("[action]")]
        public ServiceResponse InsertKafka(TestResult result)
        {
            return _service.InsertKafkaResult(result);
        }

        [HttpPost]
        [Route("[action]")]
        public ServiceResponse UpdateKafkaLastReceived(TestLastReceived args)
        {
            return _service.UpdateKafkaResult(args);
        }
    }
}
