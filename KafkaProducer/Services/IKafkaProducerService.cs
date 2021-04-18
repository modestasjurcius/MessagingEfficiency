using KafkaProducer.Entities;

namespace KafkaProducer.Services
{
    public interface IKafkaProducerService
    {
        object SendToKafka(KafkaSendMessageArgs args);
    }
}