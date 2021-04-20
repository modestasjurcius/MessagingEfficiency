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

        public RabbitSenderService()
        {
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
                    channel.QueueDeclare(
                            queue: args.Queue,
                            durable: args.Durable,
                            exclusive: args.Exclusive,
                            autoDelete: args.AutoDelete,
                            arguments: null
                        );

                    var message = FormatMessage(args.MessageByteSize);
                    var serializedMessage = JsonSerializer.SerializeToUtf8Bytes(message);

                    string guid = Guid.NewGuid().ToString();
                    message.Guid = guid;
                    var serializedMessageGuid = JsonSerializer.SerializeToUtf8Bytes(message);

                    for (int i = 0; i < args.MessageCount - 1; i++)
                    {
                        channel.BasicPublish(
                                exchange: "",
                                routingKey: args.Queue,
                                basicProperties: null,
                                body: serializedMessage
                            );
                    }

                    //last message goes with guid
                    channel.BasicPublish(
                                exchange: "",
                                routingKey: args.Queue,
                                basicProperties: null,
                                body: serializedMessageGuid
                            );

                    SendInitialTestResult(args, guid, DateTimeOffset.Now.ToUnixTimeMilliseconds());
                }

                return new ServiceResponse() { Success = true, Message = $"{args.MessageCount} messages sent" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse() { Success = false, Message = ex.Message };
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
                    MessageSize = args.MessageByteSize
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
    }
}
