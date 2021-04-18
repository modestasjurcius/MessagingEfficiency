using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaConsumer.Handlers
{
    public class KafkaConsumerHandler : IHostedService
    {
        private readonly string topic = "mytopic";
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = ConfigurationManager.AppSettings["KafkaConsumerGroup"],
                BootstrapServers = ConfigurationManager.AppSettings["BootstrapServers"], //"localhost:9092", // host.docker.internal
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var builder = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                builder.Subscribe(topic);
                var cancelToken = new CancellationTokenSource();

                try
                {
                    while (true)
                    {
                        var consumer = builder.Consume(cancellationToken);
                        if (consumer != null)
                            Console.WriteLine($"Message {consumer.Message.Value} received from {consumer.TopicPartitionOffset} ");
                    }
                }
                catch (Exception)
                {
                    builder.Close();
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
