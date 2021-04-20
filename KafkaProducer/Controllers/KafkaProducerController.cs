using KafkaProducer.Entities;
using KafkaProducer.Services;
using Microsoft.AspNetCore.Mvc;

namespace KafkaProducer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KafkaProducerController : ControllerBase
    {
        private readonly IKafkaProducerService _service;

        public KafkaProducerController(IKafkaProducerService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("[action]")]
        public ServiceResponse Get()
        {
            return new ServiceResponse() { Success = true, Message = "Kafka producer is alive" };
        }

        [HttpPost]
        [Route("[action]")]
        public ServiceResponse SendMessage(KafkaSendMessageArgs args)
        {
            return _service.SendToKafka(args);
        }
    }
}
