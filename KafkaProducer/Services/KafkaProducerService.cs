using Confluent.Kafka;
using KafkaProducer.Entities;
using RestSharp;
using System;
using System.Configuration;
using System.Text.Json;

namespace KafkaProducer.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly ProducerConfig _config;
        private readonly string _topic;

        public KafkaProducerService()
        {
            _config = new ProducerConfig()
            {
                BootstrapServers = ConfigurationManager.AppSettings["BootstrapServers"]
            };

            _topic = ConfigurationManager.AppSettings["KafkaTopic"];
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
                    var lastTimestamp = SendMessages(producer, args.MessageCount, serializedMessage, serializedMessageGuid);
                    SendInitialTestResult(args, guid, lastTimestamp);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Kafka producer error: {ex.Message}");
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

        private long SendMessages(IProducer<Null, byte[]> producer, int count, byte[] data, byte[] dataGuid)
        {
            for(int i = 0; i < count - 1; i++)
            {
                producer.ProduceAsync(_topic, new Message<Null, byte[]> { Value = data })
                        .GetAwaiter()
                        .GetResult();
            }

            producer.ProduceAsync(_topic, new Message<Null, byte[]> { Value = dataGuid })
                        .GetAwaiter()
                        .GetResult();

            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
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
                    MessageSize = args.MessageByteSize
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
