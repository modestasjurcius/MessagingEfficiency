namespace ResultsMicroservice.Entities
{
    public class RabbitTestResult
    {
        public string Guid { get; set; }
        public long SendAt { get; set; }
        public long LastReceivedAt { get; set; }
    }
}
