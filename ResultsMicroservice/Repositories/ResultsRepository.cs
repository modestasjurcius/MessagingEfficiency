using ResultsMicroservice.Entities;
using System;

namespace ResultsMicroservice.Repositories
{
    public class ResultsRepository : IResultsRepository
    {
        private readonly MySqlDatabase _database;

        public ResultsRepository(MySqlDatabase database)
        {
            _database = database;
        }

        public void InsertRabbitResult(TestResult result)
        {
            var cmd = _database.CreateCommand();
            cmd.CommandText = @"
                    INSERT INTO rabbit_results (Guid, SendAt, LastReceivedAt, MessageCount, MessageByteSize) 
                    VALUES (?guid, FROM_UNIXTIME(?sendAt * 0.001), FROM_UNIXTIME(?lastReceivedAt * 0.001), ?count, ?size)";

            cmd.Parameters.AddWithValue("?guid", result.Guid);
            cmd.Parameters.AddWithValue("?sendAt", result.SendAt);
            cmd.Parameters.AddWithValue("?lastReceivedAt", result.LastReceivedAt);
            cmd.Parameters.AddWithValue("?count", result.MessageCount);
            cmd.Parameters.AddWithValue("?size", result.MessageSize);

            cmd.ExecuteNonQuery();
        }

        public void UpdateRabbitLastReceived(TestLastReceived args)
        {
            var cmd = _database.CreateCommand();
            cmd.CommandText = @"
                    UPDATE rabbit_results 
                    SET LastReceivedAt = FROM_UNIXTIME(?receivedAt * 0.001) 
                    WHERE Guid = ?guid";

            cmd.Parameters.AddWithValue("?receivedAt", args.LastReceivedAt);
            cmd.Parameters.AddWithValue("?guid", args.Guid);

            cmd.ExecuteNonQuery();
        }

        public void InsertKafkaResult(TestResult result)
        {
            var cmd = _database.CreateCommand();
            cmd.CommandText = @"
                    INSERT INTO kafka_results (Guid, SendAt, LastReceivedAt, MessageCount, MessageByteSize) 
                    VALUES (?guid, FROM_UNIXTIME(?sendAt * 0.001), FROM_UNIXTIME(?lastReceivedAt * 0.001), ?count, ?size)";

            cmd.Parameters.AddWithValue("?guid", result.Guid);
            cmd.Parameters.AddWithValue("?sendAt", result.SendAt);
            cmd.Parameters.AddWithValue("?lastReceivedAt", result.LastReceivedAt);
            cmd.Parameters.AddWithValue("?count", result.MessageCount);
            cmd.Parameters.AddWithValue("?size", result.MessageSize);

            cmd.ExecuteNonQuery();
        }

        public void UpdateKafkaLastReceived(TestLastReceived args)
        {
            var cmd = _database.CreateCommand();
            cmd.CommandText = @"
                    UPDATE kafka_results 
                    SET LastReceivedAt = FROM_UNIXTIME(?receivedAt * 0.001) 
                    WHERE Guid = ?guid";

            cmd.Parameters.AddWithValue("?receivedAt", args.LastReceivedAt);
            cmd.Parameters.AddWithValue("?guid", args.Guid);

            cmd.ExecuteNonQuery();
        }
    }
}
