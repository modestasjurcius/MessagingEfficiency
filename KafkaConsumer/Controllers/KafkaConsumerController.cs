using KafkaConsumer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KafkaConsumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KafkaConsumerController : ControllerBase
    {
        private readonly ILogger<KafkaConsumerController> _logger;

        public KafkaConsumerController(ILogger<KafkaConsumerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        public ServiceResponse Get()
        {
            return new ServiceResponse() { Success = true, Message = "Kafka consumer is alive" };
        }
    }
}
