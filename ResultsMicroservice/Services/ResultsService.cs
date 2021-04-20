using Microsoft.Extensions.Logging;
using ResultsMicroservice.Entities;
using ResultsMicroservice.Repositories;
using System;

namespace ResultsMicroservice.Services
{
    public class ResultsService : IResultsService
    {
        private readonly ILogger<ResultsService> _logger;
        private readonly IResultsRepository _repository;

        public ResultsService(ILogger<ResultsService> logger, IResultsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public ServiceResponse InsertRabbitResult(TestResult result)
        {
            try
            {
                _logger.LogInformation($"InsertRabbitResult : args.Guid={result}");
                _repository.InsertRabbitResult(result);

                return new ServiceResponse() { Success = true, Message = "Rabbit result inserted" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"InsertRabbitResult : {ex.Message} \n {ex.InnerException}");
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }

        public ServiceResponse UpdateRabbitResult(TestLastReceived args)
        {
            try
            {
                _logger.LogInformation($"UpdateRabbitResult : args.Guid={args.Guid}, args.LastReceivedAt={args.LastReceivedAt}");
                _repository.UpdateRabbitLastReceived(args);

                return new ServiceResponse() { Success = true, Message = "Rabbit result updated" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateRabbitResult : {ex.Message} \n {ex.InnerException}");
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }

        public ServiceResponse InsertKafkaResult(TestResult result)
        {
            try
            {
                _logger.LogInformation($"InsertKafkaResult : args.Guid={result}");
                _repository.InsertKafkaResult(result);

                return new ServiceResponse() { Success = true, Message = "Kafka result inserted" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"InsertKafkaResult : {ex.Message} \n {ex.InnerException}");
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }

        public ServiceResponse UpdateKafkaResult(TestLastReceived args)
        {
            try
            {
                _logger.LogInformation($"UpdateKafkaResult : args.Guid={args.Guid}, args.LastReceivedAt={args.LastReceivedAt}");
                _repository.UpdateKafkaLastReceived(args);

                return new ServiceResponse() { Success = true, Message = "Kafka result updated" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateKafkaResult : {ex.Message} \n  {ex.InnerException}");
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }
    }
}
