namespace RabbitSenderMicroservice.Entities
{
    public class InitialTestData
    {
        public string Guid { get; set; }
        public long SendAt { get; set; }
        public int MessageCount { get; set; }
        public int MessageSize { get; set; }
        public string Topic { get; set; }
    }
}
