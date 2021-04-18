using Confluent.Kafka;
using KafkaProducer.Entities;
using System;
using System.Configuration;

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
        public object SendToKafka(KafkaSendMessageArgs args)
        {
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                try
                {
                    return producer.ProduceAsync(_topic, new Message<Null, string> { Value = args.Message })
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Kafka producer error: {ex.Message}");
                }
            }

            return null;
        }
    }
}
