using KafkaProducer.Entities;

namespace KafkaProducer.Services
{
    public interface IKafkaProducerService
    {
        ServiceResponse SendToKafka(KafkaSendMessageArgs args);
    }
}