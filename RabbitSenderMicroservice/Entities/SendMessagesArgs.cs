namespace RabbitSenderMicroservice.Entities
{
    public class SendMessagesArgs
    {
        public string Queue { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public int MessageCount { get; set; }
        public int MessageByteSize { get; set; }
    }
}
