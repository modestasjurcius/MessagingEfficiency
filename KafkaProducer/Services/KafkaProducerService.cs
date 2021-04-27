using Confluent.Kafka;
using KafkaProducer.Entities;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Configuration;
using System.Text.Json;

namespace KafkaProducer.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly ProducerConfig _config;
        private readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(ILogger<KafkaProducerService> logger)
        {
            _logger = logger;

            _config = new ProducerConfig()
            {
                BootstrapServers = ConfigurationManager.AppSettings["BootstrapServers"],
                TransactionalId = ConfigurationManager.AppSettings["KafkaTransactionalId"]
            };
        }

        public ServiceResponse SendToKafka(KafkaSendMessageArgs args)
        {
            var guid = Guid.NewGuid().ToString();
            var message = GetMessage(args);
            var serializedMessage = JsonSerializer.SerializeToUtf8Bytes(message);

            message.Guid = guid;
            var serializedMessageGuid = JsonSerializer.SerializeToUtf8Bytes(message);

            using (var producer = new ProducerBuilder<Null, byte[]>(_config).Build())
            {
                try
                {
                    SendMessages(producer, args, serializedMessage, serializedMessageGuid, guid);

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Kafka producer error: {ex.Message}");
                    return new ServiceResponse() { Success = false, Message = ex.Message };
                }
            }

            return new ServiceResponse() { Success = true, Message = $"{args.MessageCount} messages sent to kafka" };
        }

        private KafkaMessage GetMessage(KafkaSendMessageArgs args)
        {
            var data = string.Empty;
            while (data.Length < args.MessageByteSize)
                data += "a";

            return new KafkaMessage
            {
                Data = data,
            };
        }

        private void SendMessages(IProducer<Null, byte[]> producer, KafkaSendMessageArgs args, byte[] data, byte[] dataGuid, string guid)
        {
            producer.InitTransactions(TimeSpan.FromSeconds(10));
            producer.BeginTransaction();

            for(int i = 0; i < args.MessageCount - 1; i++)
            {
                producer.Produce(args.Topic, new Message<Null, byte[]> { Value = data });
            }

            producer.Produce(args.Topic, new Message<Null, byte[]> { Value = dataGuid });

            producer.CommitTransaction();

            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            SendInitialTestResult(args, guid, now);
            _logger.LogInformation($"Kafka sender: Guid = {guid} - Sent at: {DateTimeOffset.FromUnixTimeMilliseconds(now).AddHours(3).ToString("yyyy-MM-dd hh:mm:ss.fff tt")}"); //cuz im in +0300 atm
        }

        private void SendInitialTestResult(KafkaSendMessageArgs args, string guid, long timestamp)
        {
            try
            {
                var client = new RestClient(ConfigurationManager.AppSettings["ResultsUrl"]);
                var request = new RestRequest("/Results/InsertKafka", Method.POST);

                var sendData = new InitialTestData()
                {
                    Guid = guid,
                    SendAt = timestamp,
                    MessageCount = args.MessageCount,
                    MessageSize = args.MessageByteSize,
                    Topic = args.Topic,
                };

                request.AddJsonBody(sendData);

                var response = client.Execute<ServiceResponse>(request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"KafkaProducer.SendInitialTestResult error: {ex.Message}");
            }
        }
    }
}
