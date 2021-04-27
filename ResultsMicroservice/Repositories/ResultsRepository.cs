using MySql.Data.MySqlClient;
using ResultsMicroservice.Entities;
using System;
using System.Configuration;

namespace ResultsMicroservice.Repositories
{
    public class ResultsRepository : IResultsRepository
    {
        private readonly string _connectionString;

        public ResultsRepository()
        {
            _connectionString = ConfigurationManager.AppSettings["connectionString"];
        }

        public void InsertRabbitResult(TestResult result)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT IGNORE INTO rabbit_results (Guid, SendAt, LastReceivedAt, MessageCount, MessageByteSize) 
                    VALUES (?guid, FROM_UNIXTIME(?sendAt * 0.001), FROM_UNIXTIME(?lastReceivedAt * 0.001), ?count, ?size)
                    ON DUPLICATE KEY UPDATE
                    SendAt = FROM_UNIXTIME(?sendAt * 0.001),
                    MessageCount = ?count,
                    MessageByteSize = ?size;";

                cmd.Parameters.AddWithValue("?guid", result.Guid);
                cmd.Parameters.AddWithValue("?sendAt", result.SendAt);
                cmd.Parameters.AddWithValue("?lastReceivedAt", result.LastReceivedAt);
                cmd.Parameters.AddWithValue("?count", result.MessageCount);
                cmd.Parameters.AddWithValue("?size", result.MessageSize);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateRabbitLastReceived(TestLastReceived args)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT IGNORE INTO rabbit_results (Guid, SendAt, LastReceivedAt, MessageCount, MessageByteSize) 
                    VALUES (?guid, null, FROM_UNIXTIME(?lastReceivedAt * 0.001), null, null)
                    ON DUPLICATE KEY UPDATE
                    LastReceivedAt = FROM_UNIXTIME(?lastReceivedAt * 0.001);";

                cmd.Parameters.AddWithValue("?lastReceivedAt", args.LastReceivedAt);
                cmd.Parameters.AddWithValue("?guid", args.Guid);

                cmd.ExecuteNonQuery();
            }
        }

        public void InsertKafkaResult(TestResult result)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT IGNORE INTO kafka_results_v2 (Guid, SendAt, LastReceivedAt, MessageCount, MessageByteSize, Topic) 
                    VALUES (?guid, FROM_UNIXTIME(?sendAt * 0.001), FROM_UNIXTIME(?lastReceivedAt * 0.001), ?count, ?size, ?topic)
                    ON DUPLICATE KEY UPDATE
                    SendAt = FROM_UNIXTIME(?sendAt * 0.001),
                    MessageCount = ?count,
                    MessageByteSize = ?size,
                    Topic = ?topic;";

                cmd.Parameters.AddWithValue("?guid", result.Guid);
                cmd.Parameters.AddWithValue("?sendAt", result.SendAt);
                cmd.Parameters.AddWithValue("?lastReceivedAt", result.LastReceivedAt);
                cmd.Parameters.AddWithValue("?count", result.MessageCount);
                cmd.Parameters.AddWithValue("?size", result.MessageSize);
                cmd.Parameters.AddWithValue("?topic", result.Topic);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateKafkaLastReceived(TestLastReceived args)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT IGNORE INTO kafka_results_v2 (Guid, SendAt, LastReceivedAt, MessageCount, MessageByteSize, Topic) 
                    VALUES (?guid, null, FROM_UNIXTIME(?lastReceivedAt * 0.001), null, null, null)
                    ON DUPLICATE KEY UPDATE
                    LastReceivedAt = FROM_UNIXTIME(?lastReceivedAt * 0.001);";

                cmd.Parameters.AddWithValue("?lastReceivedAt", args.LastReceivedAt);
                cmd.Parameters.AddWithValue("?guid", args.Guid);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
