namespace RabbitConsumerMicroservice.Entities
{
    public class RabbitLastMessageReceivedArgs
    {
        public string Guid { get; set; }
        public long LastReceivedAt { get; set; }
    }
}
