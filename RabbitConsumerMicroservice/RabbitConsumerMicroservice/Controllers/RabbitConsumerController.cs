using Microsoft.AspNetCore.Mvc;
using RabbitConsumerMicroservice.Entities;
using RabbitConsumerMicroservice.Services;

namespace RabbitConsumerMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RabbitConsumerController : ControllerBase
    {
        private readonly IRabbitConsumerService _service;

        public RabbitConsumerController(IRabbitConsumerService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("[action]")]
        public ServiceResponse Consume()
        {
            return _service.Consume();
        }

        [HttpGet]
        [Route("[action]")]
        public ServiceResponse Get()
        {
            return _service.Get();
        }
    }
}
