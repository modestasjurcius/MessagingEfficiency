namespace ResultsMicroservice.Entities
{
    public class RabbitTestResult : RabbitTestLastReceived
    {
        public long SendAt { get; set; }
        public int MessageCount { get; set; }
        public int MessageSize { get; set; }
    }
}
