using RabbitConsumerMicroservice.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;
using System;
using System.Configuration;
using System.Text;
using System.Text.Json;

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
                    if (body.Length == 0)
                        return;

                    var readOnlySpan = new ReadOnlySpan<byte>(body);
                    var message = JsonSerializer.Deserialize<RabbitMessage>(readOnlySpan);

                    if (message != null && !string.IsNullOrWhiteSpace(message.Guid))
                        UpdateLastReceived(DateTimeOffset.Now.ToUnixTimeMilliseconds(), message.Guid);
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

        private void UpdateLastReceived(long receivedAt, string guid)
        {
            try
            {
                var client = new RestClient(ConfigurationManager.AppSettings["ResultsUrl"]);
                var request = new RestRequest("/Results/UpdateLastReceived", Method.POST);

                var sendData = new RabbitLastMessageReceivedArgs()
                {
                    Guid = guid,
                    LastReceivedAt = receivedAt
                };

                request.AddJsonBody(sendData);

                var response = client.Execute<ServiceResponse>(request);
            }
            catch (Exception)
            {
            }
        }

        private void ConfigureConsumer()
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = ConfigurationManager.AppSettings["RabbitHostName"] };
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
            catch(Exception)
            {
            }
        }
    }
}
