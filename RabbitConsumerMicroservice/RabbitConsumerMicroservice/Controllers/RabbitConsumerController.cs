using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitConsumerMicroservice.Entities;
using RabbitConsumerMicroservice.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public ServiceResponse Get()
        {
            return _service.Get();
        }
    }
}
