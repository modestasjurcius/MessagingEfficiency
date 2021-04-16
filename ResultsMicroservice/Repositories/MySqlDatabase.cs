using MySql.Data.MySqlClient;
using System;

namespace ResultsMicroservice.Repositories
{
    public class MySqlDatabase : IDisposable
    {
        private MySqlConnection _connection;

        public MySqlDatabase(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
        }

        public MySqlCommand CreateCommand()
        {
            return _connection.CreateCommand();
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
