using Microsoft.Extensions.Logging;
using RabbitConsumerMicroservice.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestSharp;
using System;
using System.Configuration;
using System.Text.Json;

namespace RabbitConsumerMicroservice.Services
{
    public class RabbitConsumerService : IRabbitConsumerService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger<RabbitConsumerService> _logger;
        public RabbitConsumerService(ILogger<RabbitConsumerService> logger)
        {
            _logger = logger;
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
                    HandleMessage(ea);
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
                _logger.LogError($"Consume: {ex.Message}");
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }

        private void HandleMessage(BasicDeliverEventArgs args)
        {
            try
            {
                var body = args.Body.ToArray();
                if (body.Length == 0)
                    return;

                var readOnlySpan = new ReadOnlySpan<byte>(body);
                var message = JsonSerializer.Deserialize<RabbitMessage>(readOnlySpan);

                if (message == null)
                {
                    return;
                }
                else if (message.Data == "first")
                {
                    _logger.LogInformation($"First message received at: {DateTimeOffset.Now.AddHours(3).ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                }
                else if (!string.IsNullOrWhiteSpace(message.Guid))
                {
                    var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    _logger.LogInformation($"Last message, guid: {message.Guid}, received at: {DateTimeOffset.FromUnixTimeMilliseconds(now).AddHours(3).ToString("yyyy-MM-dd HH:mm:ss.fff")}"); // +0300
                    UpdateLastReceived(now, message.Guid);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"HandleMessage: {ex.Message}");
            }
        }

        private void UpdateLastReceived(long receivedAt, string guid)
        {
            try
            {
                var client = new RestClient(ConfigurationManager.AppSettings["ResultsUrl"]);
                var request = new RestRequest("/Results/UpdateRabbitLastReceived", Method.POST);

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
            catch(Exception ex)
            {
                _logger.LogError($"ConfigureConsumer: {ex.Message}");
            }
        }
    }
}
