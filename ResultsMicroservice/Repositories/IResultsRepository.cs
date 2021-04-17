using ResultsMicroservice.Entities;

namespace ResultsMicroservice.Repositories
{
    public interface IResultsRepository
    {
        void InsertRabbitResult(RabbitTestResult result);
        void UpdateLastReceived(RabbitTestLastReceived args);
    }
}