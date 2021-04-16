using RabbitMQ.Client;
using RabbitSenderMicroservice.Entities;
using RestSharp;
using System;
using System.Configuration;
using System.Text;

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
            // hostname = 'localhost' for local debug, 'host.docker.internal' for docker
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

                    string guid = Guid.NewGuid().ToString();
                    var body = Encoding.UTF8.GetBytes(guid);

                    for (int i = 0; i < args.MessageCount - 1; i++)
                    {
                        channel.BasicPublish(
                                exchange: "",
                                routingKey: args.Queue,
                                basicProperties: null,
                                body: null
                            );
                    }

                    channel.BasicPublish(
                                exchange: "",
                                routingKey: args.Queue,
                                basicProperties: null,
                                body: body
                            );

                    SendInitialTestResult(guid, DateTimeOffset.Now.ToUnixTimeMilliseconds());
                }

                return new ServiceResponse() { Success = true, Message = $"{args.MessageCount} messages sent" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }

        private void SendInitialTestResult(string guid, long timestamp)
        {
            try
            {
                var client = new RestClient(ConfigurationManager.AppSettings["ResultsUrl"]);
                var request = new RestRequest("/Results/Insert", Method.POST);

                var sendData = new InitialTestData()
                {
                    Guid = guid,
                    SendAt = timestamp
                };

                request.AddJsonBody(sendData);

                var response = client.Execute<ServiceResponse>(request);
            }
            catch (Exception)
            {

            }
        }
    }
}
