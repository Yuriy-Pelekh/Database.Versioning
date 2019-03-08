using NUnit.Framework;

namespace Database.Versioning.Tests
{
    public class DatabaseTest
    {
        [Test, Category(TestCategory.LongRunning)]
        public void DatabaseExistsTest()
        {
            const string connectionString = "Data Source=(local);Integrated Security=True";
            var target = new Database(connectionString);
            var databaseExists = target.Exists("Test");
            Assert.False(databaseExists);
        }
    }
}
