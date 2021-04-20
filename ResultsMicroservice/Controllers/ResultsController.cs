using Microsoft.AspNetCore.Mvc;
using ResultsMicroservice.Entities;
using ResultsMicroservice.Services;

namespace ResultsMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly IResultsService _service;

        public ResultsController(IResultsService service)
        {
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
