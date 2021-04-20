using Confluent.Kafka;
using KafkaConsumer.Entities;
using Microsoft.Extensions.Hosting;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.Json;
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

            using (var builder = new ConsumerBuilder<Ignore, byte[]>(config).Build())
            {
                builder.Subscribe(topic);
                var cancelToken = new CancellationTokenSource();

                try
                {
                    while (true)
                    {
                        var consumer = builder.Consume(cancellationToken);
                        if (consumer != null)
                        {
                            var readOnlySpan = new ReadOnlySpan<byte>(consumer.Message.Value);
                            var message = JsonSerializer.Deserialize<KafkaMessage>(readOnlySpan);

                            if (!string.IsNullOrWhiteSpace(message.Guid))
                                UpdateLastReceived(message.Guid, DateTimeOffset.Now.ToUnixTimeMilliseconds());
                        }
                    }
                }
                catch (Exception)
                {
                    builder.Close();
                }
            }

            return Task.CompletedTask;
        }

        private void UpdateLastReceived(string guid, long receivedAt)
        {
            try
            {
                var client = new RestClient(ConfigurationManager.AppSettings["ResultsUrl"]);
                var request = new RestRequest("/Results/UpdateKafkaLastReceived", Method.POST);

                var sendData = new KafkaLastMessageReceivedArgs()
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
