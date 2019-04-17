using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace Database.Versioning
{
    public class DatabaseManager
    {
        private const string SqlCreateFileName = @"Scripts/create.sql";
        private const string SqlUpdateFileName = @"Scripts/update.sql";
        private readonly string _connectionString;

        public DatabaseManager(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Value cannot be null or empty string", nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public bool Exists()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            var databaseName = builder.InitialCatalog;
            builder.InitialCatalog = string.Empty;

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                return Exists(connection, databaseName);
            }
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

        private static int GetVersion(SqlConnection connection)
        {
            const string sql = "SELECT MAX([Version]) FROM [Version]";
            var command = new SqlCommand(sql, connection);
            var version = command.ExecuteScalar();
            return Convert.ToInt32(version);
        }

        public void Create()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            var databaseName = builder.InitialCatalog;
            builder.InitialCatalog = string.Empty;

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                ExecuteScriptFromFile(connection, SqlCreateFileName, new KeyValuePair<string, string>("{database}", databaseName));
            }
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

        public void Update()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                Update(connection);
            }
        }

        private void Update(SqlConnection connection)
        {
            const string sql = "INSERT INTO [Version] ([Version], [UpdatedDate]) VALUES(@version, GETUTCDATE())";
            var currentVersion = GetVersion(connection);
            var script = File.ReadAllText(SqlUpdateFileName);
            var versions = script.Split(new[] {"--##"}, StringSplitOptions.RemoveEmptyEntries);
            var lineEnding = Environment.NewLine;

            foreach (var versionScript in versions)
            {
                if (versionScript.Trim() != string.Empty)
                {
                    var indexOfVersionNumber = versionScript.IndexOf(lineEnding, StringComparison.InvariantCulture);
                    if (indexOfVersionNumber == -1)
                    {
                        lineEnding = "\n";
                        indexOfVersionNumber = versionScript.IndexOf(lineEnding, StringComparison.InvariantCulture);
                    }

                    var version = Convert.ToInt32(versionScript.Substring(0, indexOfVersionNumber));

                    if (currentVersion < version)
                    {
                        var commandString = versionScript.Substring(indexOfVersionNumber + lineEnding.Length).Trim();

                        ExecuteScript(connection, commandString);

                        var command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@version", version);
                        command.ExecuteNonQuery();
                    }
                }
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
