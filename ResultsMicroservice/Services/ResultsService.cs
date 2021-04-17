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

        public ServiceResponse InsertRabbitResult(RabbitTestResult result)
        {
            try
            {
                _repository.InsertRabbitResult(result);

                return new ServiceResponse() { Success = true, Message = "Inserted" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }

        public ServiceResponse UpdateRabbitResult(RabbitTestLastReceived args)
        {
            try
            {
                _repository.UpdateLastReceived(args);

                return new ServiceResponse() { Success = true, Message = "Updated" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse() { Success = false, Message = ex.Message };
            }
        }
    }
}
