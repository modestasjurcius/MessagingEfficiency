namespace KafkaProducer.Entities
{
    public class KafkaSendMessageArgs
    {
        public string Topic { get; set; }
        public int MessageCount { get; set; }
        public int MessageByteSize { get; set; }
    }
}
