namespace KafkaConsumer.Entities
{
    public class KafkaLastMessageReceivedArgs
    {
        public string Guid { get; set; }
        public long LastReceivedAt { get; set; }
    }
}
