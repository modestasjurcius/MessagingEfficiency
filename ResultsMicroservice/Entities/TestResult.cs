namespace ResultsMicroservice.Entities
{
    public class TestResult : TestLastReceived
    {
        public long SendAt { get; set; }
        public int MessageCount { get; set; }
        public int MessageSize { get; set; }
        public string Topic { get; set; }
    }
}
