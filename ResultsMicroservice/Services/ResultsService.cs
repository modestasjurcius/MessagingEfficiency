using ResultsMicroservice.Entities;
using ResultsMicroservice.Repositories;
using System;

namespace ResultsMicroservice.Services
{
    public class ResultsService : IResultsService
    {
        private readonly IResultsRepository _repository;

        public ResultsService(IResultsRepository repository)
        {
            _repository = repository;
        }

        public ServiceResponse InsertRabbitResult(TestResult result)
        {
            try
            {
                _repository.InsertRabbitResult(result);

                return new ServiceResponse() { Success = true, Message = "Rabbit result inserted" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }

        public ServiceResponse UpdateRabbitResult(TestLastReceived args)
        {
            try
            {
                _repository.UpdateRabbitLastReceived(args);

                return new ServiceResponse() { Success = true, Message = "Rabbit result updated" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }

        public ServiceResponse InsertKafkaResult(TestResult result)
        {
            try
            {
                _repository.InsertKafkaResult(result);

                return new ServiceResponse() { Success = true, Message = "Kafka result inserted" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }

        public ServiceResponse UpdateKafkaResult(TestLastReceived args)
        {
            try
            {
                _repository.UpdateKafkaLastReceived(args);

                return new ServiceResponse() { Success = true, Message = "Kafka result updated" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }
    }
}
