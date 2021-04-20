using ResultsMicroservice.Entities;

namespace ResultsMicroservice.Services
{
    public interface IResultsService
    {
        ServiceResponse InsertRabbitResult(TestResult result);
        ServiceResponse UpdateRabbitResult(TestLastReceived args);
        ServiceResponse InsertKafkaResult(TestResult result);
        ServiceResponse UpdateKafkaResult(TestLastReceived args);
    }
}