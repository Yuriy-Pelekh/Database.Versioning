using System;
using System.Data.SqlClient;

namespace Database.Versioning
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Exists(string databaseName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return Exists(connection, databaseName);
            }
        }

        public bool Exists(SqlConnection connection, string databaseName)
        {
            var command = new SqlCommand("select DB_ID(@database)", connection);
            command.Parameters.AddWithValue("@database", databaseName);
            var executeScalar = command.ExecuteScalar();
            return executeScalar != DBNull.Value;
        }
    }
}
