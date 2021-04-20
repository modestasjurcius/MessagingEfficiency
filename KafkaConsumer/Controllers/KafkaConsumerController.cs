using KafkaConsumer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KafkaConsumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KafkaConsumerController : ControllerBase
    {
        public KafkaConsumerController()
        {
        }

        [HttpGet]
        [Route("[action]")]
        public ServiceResponse Get()
        {
            return new ServiceResponse() { Success = true, Message = "Kafka consumer is alive" };
        }
    }
}
