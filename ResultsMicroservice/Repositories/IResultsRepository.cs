using ResultsMicroservice.Entities;

namespace ResultsMicroservice.Repositories
{
    public interface IResultsRepository
    {
        void InsertRabbitResult(TestResult result);
        void UpdateRabbitLastReceived(TestLastReceived args);
        void InsertKafkaResult(TestResult result);
        void UpdateKafkaLastReceived(TestLastReceived args);
    }
}