using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace Database.Versioning
{
    public class Database
    {
        private const string SqlCreateFileName = @"Scripts\create.sql";
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

        protected bool Exists(SqlConnection connection, string databaseName)
        {
            var command = new SqlCommand("select DB_ID(@database)", connection);
            command.Parameters.AddWithValue("@database", databaseName);
            var executeScalar = command.ExecuteScalar();
            return executeScalar != DBNull.Value;
        }

        public void Create(string databaseName)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString) {InitialCatalog = string.Empty};

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                ExecuteScriptFromFile(connection, SqlCreateFileName, new KeyValuePair<string, string>("{database}", databaseName));
            }
        }

        protected void ExecuteScriptFromFile(SqlConnection connection, string fileName, params SqlParameter[] parameters)
        {
            var script = File.ReadAllText(fileName);
            ExecuteScript(connection, script, parameters);
        }

        protected void ExecuteScriptFromFile(SqlConnection connection, string fileName, params KeyValuePair<string, string>[] replaceParameters)
        {
            var script = File.ReadAllText(fileName);

            foreach (var replaceParameter in replaceParameters)
            {
                script = script.Replace(replaceParameter.Key, replaceParameter.Value);
            }

            ExecuteScript(connection, script);
        }

        public void ExecuteScript(string script)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                ExecuteScript(connection, script);
            }
        }

        protected void ExecuteScript(SqlConnection connection, string script, params SqlParameter[] parameters)
        {
            var commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (var commandString in commandStrings)
            {
                if (commandString.Trim() != string.Empty)
                {
                    using (var command = new SqlCommand(commandString, connection))
                    {
                        foreach (var sqlParameter in parameters)
                        {
                            command.Parameters.AddWithValue(sqlParameter.ParameterName, sqlParameter.Value);
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
