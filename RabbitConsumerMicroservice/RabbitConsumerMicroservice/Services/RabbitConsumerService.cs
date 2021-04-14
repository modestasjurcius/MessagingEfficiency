using RabbitConsumerMicroservice.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Configuration;
using System.Text;

namespace RabbitConsumerMicroservice.Services
{
    public class RabbitConsumerService : IRabbitConsumerService
    {
        public RabbitConsumerService()
        {
            ConfigureConsumer();
        }
        
        public ServiceResponse Get()
        {
            return new ServiceResponse() { Success = true, Message = "Alive" };
        }

        private void ConfigureConsumer()
        {
            try
            {
                // hostname = 'localhost' for local debug, 'host.docker.internal' for docker
                var factory = new ConnectionFactory() { HostName = ConfigurationManager.AppSettings["RabbitHostName"] };
                factory.UserName = ConfigurationManager.AppSettings["RabbitUsername"];
                factory.Password = ConfigurationManager.AppSettings["RabbitPassword"];
                factory.Port = int.Parse(ConfigurationManager.AppSettings["RabbitPort"]);

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                            queue: ConfigurationManager.AppSettings["Queue"],
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null
                        );

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                    };

                    channel.BasicConsume(
                            queue: ConfigurationManager.AppSettings["Queue"],
                            autoAck: true,
                            consumer: consumer
                        );
                }
            }
            catch(Exception ex)
            {
                //report exception to results service
            }
        }
    }
}
