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

        public void InsertRabbitResult(RabbitTestResult result)
        {
            var cmd = _database.CreateCommand();
            cmd.CommandText = "INSERT INTO rabbit_results (Guid, SendAt, LastReceivedAt) VALUES (?guid, FROM_UNIXTIME(?sendAt * 0.001), FROM_UNIXTIME(?lastReceivedAt * 0.001))";
            cmd.Parameters.AddWithValue("?guid", result.Guid);
            cmd.Parameters.AddWithValue("?sendAt", result.SendAt);
            cmd.Parameters.AddWithValue("?lastReceivedAt", result.LastReceivedAt);

            cmd.ExecuteNonQuery();
        }
    }
}
