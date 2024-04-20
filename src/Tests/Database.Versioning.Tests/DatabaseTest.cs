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
            Assert.That(target, Is.InstanceOf<DatabaseManager>());
        }

        [Test, Category(TestCategory.LongRunning), Ignore("Cannot run on CI")]
        public void DatabaseExistsTest()
        {
            const string connectionString = "Data Source=(local);Integrated Security=True";
            var target = new DatabaseManager(connectionString);
            var databaseExists = target.Exists("Test");
            Assert.That(databaseExists, Is.False);
        }

        [Test, Category(TestCategory.LongRunning), Ignore("Cannot run on CI")]
        public void DatabaseCreateTest()
        {
            string connectionString = "Data Source=(local);Integrated Security=True";
            var databaseName = "Test_" + DateTime.UtcNow.Ticks;

            var target = new DatabaseManager(connectionString);
            Assert.That(target.Exists(databaseName), Is.False);
            target.Create(databaseName);
            Assert.That(target.Exists(databaseName), Is.True);

            connectionString = $"Data Source=(local);Initial Catalog={databaseName};Integrated Security=True";
            target = new DatabaseManager(connectionString);
            target.Update();
        }
    }
}
