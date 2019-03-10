using System;
using NUnit.Framework;

namespace Database.Versioning.Tests
{
    public class DatabaseTest
    {
        [Test, Category(TestCategory.UnitTest)]
        public void CreateDatabaseManagerObjectTest()
        {
            Assert.Throws<ArgumentException>(() => new DatabaseManager(null));
            Assert.Throws<ArgumentException>(() => new DatabaseManager(string.Empty));
            Assert.Throws<ArgumentException>(() => new DatabaseManager(" "));

            var target = new DatabaseManager("connection string goes here");
            Assert.IsInstanceOf<DatabaseManager>(target);
        }

        [Test, Category(TestCategory.LongRunning), Ignore("Cannot run on CI")]
        public void DatabaseExistsTest()
        {
            const string connectionString = "Data Source=(local);Integrated Security=True";
            var target = new DatabaseManager(connectionString);
            var databaseExists = target.Exists("Test");
            Assert.False(databaseExists);
        }

        [Test, Category(TestCategory.LongRunning), Ignore("Cannot run on CI")]
        public void DatabaseCreateTest()
        {
            string connectionString = "Data Source=(local);Integrated Security=True";
            var databaseName = "Test_" + DateTime.UtcNow.Ticks;

            var target = new DatabaseManager(connectionString);
            Assert.False(target.Exists(databaseName));
            target.Create(databaseName);
            Assert.True(target.Exists(databaseName));

            connectionString = $"Data Source=(local);Initial Catalog={databaseName};Integrated Security=True";
            target = new DatabaseManager(connectionString);
            target.Update();
        }
    }
}
