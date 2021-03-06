using Confluent.Kafka;
using KafkaConsumer.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<KafkaConsumerHandler> _logger;

        public KafkaConsumerHandler(ILogger<KafkaConsumerHandler> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = ConfigurationManager.AppSettings["KafkaConsumerGroup"],
                BootstrapServers = ConfigurationManager.AppSettings["BootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var builder = new ConsumerBuilder<Ignore, byte[]>(config).Build())
            {
                builder.Subscribe(ConfigurationManager.AppSettings["KafkaTopic"]);
                var cancelToken = new CancellationTokenSource();

                bool first = true;

                try
                {
                    while (true)
                    {
                        var consumer = builder.Consume(cancellationToken);
                        if (consumer != null)
                        {
                            var readOnlySpan = new ReadOnlySpan<byte>(consumer.Message.Value);
                            var message = JsonSerializer.Deserialize<KafkaMessage>(readOnlySpan);

                            if (first)
                            {
                                _logger.LogInformation($"First message, offset = {consumer.Offset} received at: {DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt")}");
                                first = false;
                            }

                            if (message != null && !string.IsNullOrWhiteSpace(message.Guid))
                            {
                                var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                UpdateLastReceived(message.Guid, DateTimeOffset.Now.ToUnixTimeMilliseconds());
                                _logger.LogInformation($"Last message, offset = {consumer.Offset}, guid: {message.Guid}, received at: {DateTimeOffset.FromUnixTimeMilliseconds(now).ToString("yyyy-MM-dd hh:mm:ss.fff tt")}"); // +0300
                                first = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"StartAsync: {ex.Message}");
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
                _logger.LogInformation($"UpdateLastReceived: response status: {response.StatusCode} , error: {response.ErrorMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateLastReceived: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
