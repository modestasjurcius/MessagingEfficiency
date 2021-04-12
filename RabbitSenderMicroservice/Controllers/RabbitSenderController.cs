using Microsoft.AspNetCore.Mvc;
using RabbitSenderMicroservice.Entities;
using RabbitSenderMicroservice.Services;

namespace RabbitSenderMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RabbitSenderController : ControllerBase
    {
        
        private readonly IRabbitSenderService _service;

        public RabbitSenderController(IRabbitSenderService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("[action]")]
        public ServiceResponse Get()
        {
            return _service.Get();
        }

        [HttpPost]
        [Route("[action]")]
        public ServiceResponse SendMessages(SendMessagesArgs args)
        {
            return _service.SendMessages(args);
        }
    }
}
