using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitSenderMicroservice.Entities;
using RestSharp;
using System;
using System.Configuration;
using System.Text;
using System.Text.Json;

namespace RabbitSenderMicroservice.Services
{
    public class RabbitSenderService : IRabbitSenderService
    {
        private IConnection _connection;
        private readonly ILogger<RabbitSenderService> _logger;

        public RabbitSenderService(ILogger<RabbitSenderService> logger)
        {
            _logger = logger;
            Configure();
        }

        private void Configure()
        {
            var factory = new ConnectionFactory() { HostName = ConfigurationManager.AppSettings["RabbitHostName"] };
            factory.UserName = ConfigurationManager.AppSettings["RabbitUsername"];
            factory.Password = ConfigurationManager.AppSettings["RabbitPassword"];
            factory.Port = int.Parse(ConfigurationManager.AppSettings["RabbitPort"]);

            _connection = factory.CreateConnection();
        }

        public ServiceResponse Get()
        {
            return new ServiceResponse() { Success = true, Message = "Rabbit sender is alive" };
        }

        public ServiceResponse SendMessages(SendMessagesArgs args)
        {
            try
            {
                using (var channel = _connection.CreateModel())
                {
                    SendMessages(channel, args);
                }

                return new ServiceResponse() { Success = true, Message = $"{args.MessageCount} messages, of size {args.MessageByteSize} bytes, sent to queue {args.Queue}" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }

        private void SendMessages(IModel channel, SendMessagesArgs args)
        {
            try
            {
                channel.QueueDeclare(
                                queue: args.Queue,
                                durable: args.Durable,
                                exclusive: args.Exclusive,
                                autoDelete: args.AutoDelete,
                                arguments: null
                            );

                int loopCount = args.MessageCount;

                byte[] firstMessageData = null;
                if (args.FlagFirstMessage)
                {
                    var firstMessage = GetFirstMessage();
                    firstMessageData = JsonSerializer.SerializeToUtf8Bytes(firstMessage);
                    loopCount -= 2;
                }
                else
                {
                    loopCount -= 1;
                }

                var message = FormatMessage(args.MessageByteSize);
                var serializedMessage = JsonSerializer.SerializeToUtf8Bytes(message);

                string guid = Guid.NewGuid().ToString();
                message.Guid = guid;
                var serializedMessageGuid = JsonSerializer.SerializeToUtf8Bytes(message);

                if (firstMessageData != null)
                {
                    channel.BasicPublish(
                            exchange: "",
                            routingKey: args.Queue,
                            basicProperties: null,
                            body: firstMessageData
                        );
                }

                if (loopCount > 0)
                {
                    for (int i = 0; i < loopCount; i++)
                    {
                        channel.BasicPublish(
                                exchange: "",
                                routingKey: args.Queue,
                                basicProperties: null,
                                body: serializedMessage
                            );
                    }
                }

                //last message goes with guid
                channel.BasicPublish(
                            exchange: "",
                            routingKey: args.Queue,
                            basicProperties: null,
                            body: serializedMessageGuid
                        );

                var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                _logger.LogInformation($"Rabbit sender: Guid = {guid} - Sent at: {DateTimeOffset.FromUnixTimeMilliseconds(now).AddHours(3).ToString("yyyy-MM-dd HH:mm:ss.fff")}"); //cuz im in +0300 atm
                SendInitialTestResult(args, guid, now);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SendMessages: {ex.Message}");
            }
        }

        private void SendInitialTestResult(SendMessagesArgs args, string guid, long timestamp)
        {
            try
            {
                var client = new RestClient(ConfigurationManager.AppSettings["ResultsUrl"]);
                var request = new RestRequest("/Results/InsertRabbit", Method.POST);

                var sendData = new InitialTestData()
                {
                    Guid = guid,
                    SendAt = timestamp,
                    MessageCount = args.MessageCount,
                    MessageSize = args.MessageByteSize,
                    Topic = args.Queue,
                };

                request.AddJsonBody(sendData);

                var response = client.Execute<ServiceResponse>(request);
            }
            catch (Exception)
            {
            }
        }

        private RabbitMessage FormatMessage(int byteCount)
        {
            var data = "";
            var bytes = Encoding.UTF8.GetBytes(data);

            while (bytes.Length < byteCount)
            {
                data += "a";
                bytes = Encoding.UTF8.GetBytes(data);
            }

            return new RabbitMessage
            {
                Data = data,
                Guid = string.Empty
            };
        }

        private RabbitMessage GetFirstMessage()
        {
            return new RabbitMessage()
            {
                Data = "first",
                Guid = string.Empty
            };
        }
    }
}
