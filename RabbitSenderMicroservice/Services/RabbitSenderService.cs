using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitSenderMicroservice.Entities;
using System;
using System.Text;

namespace RabbitSenderMicroservice.Services
{
    public class RabbitSenderService : IRabbitSenderService
    {
        private readonly ILogger<RabbitSenderService> _logger;

        public RabbitSenderService(ILogger<RabbitSenderService> logger)
        {
            _logger = logger;
        }

        public ServiceResponse Get()
        {
            return new ServiceResponse() { Success = true, Message = "Alive" };
        }

        public ServiceResponse SendMessages(SendMessagesArgs args)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                            queue: args.Queue,
                            durable: args.Durable,
                            exclusive: args.Exclusive,
                            autoDelete: args.AutoDelete,
                            arguments: null
                        );

                    string message = "Hello from RabbitSenderService!";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                            exchange: "",
                            routingKey: args.Queue,
                            basicProperties: null,
                            body: body
                        );
                }

                return new ServiceResponse() { Success = true, Message = "Messages sent" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }
    }
}
