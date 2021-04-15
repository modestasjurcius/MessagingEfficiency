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
        private IConnection _connection;
        private IModel _channel;
        public RabbitConsumerService()
        {
            ConfigureConsumer();
        }
        
        public ServiceResponse Get()
        {
            return new ServiceResponse() { Success = true, Message = "Rabbit consumer is alive" };
        }

        public ServiceResponse Consume()
        {
            try
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                };

                _channel.BasicConsume(
                        queue: ConfigurationManager.AppSettings["Queue"],
                        autoAck: true,
                        consumer: consumer
                    );

                return new ServiceResponse() { Success = true, Message = "Rabbit consumer started working" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }

        private void ConfigureConsumer()
        {
            try
            {
                // hostname = 'localhost' for local debug, 'host.docker.internal' for docker
                var factory = new ConnectionFactory() { HostName = ConfigurationManager.AppSettings["RabbitHostName"] };
                //var factory = new ConnectionFactory() { HostName = "localhost" };
                factory.UserName = ConfigurationManager.AppSettings["RabbitUsername"];
                factory.Password = ConfigurationManager.AppSettings["RabbitPassword"];
                factory.Port = int.Parse(ConfigurationManager.AppSettings["RabbitPort"]);

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                            queue: ConfigurationManager.AppSettings["Queue"],
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null
                        );
            }
            catch(Exception ex)
            {
                //report exception to results service
            }
        }
    }
}
