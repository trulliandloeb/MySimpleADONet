using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimplaAdoNet.Entity;

namespace SimpleAdoNet.Test
{
    [TestFixture]
    public class EasySQLerTest
    {
        private const string CONN_STRING = "data source=\"D:\\Program Files (x86)\\SQLite.NET\\db\\mysqlo\"";
        private EasySQLer sqler;

        [SetUp]
        public void SetUp()
        {
            sqler = new EasySQLer(CONN_STRING);
        }

        [Test]
        public void TestScalar()
        {
            var totalCount = sqler.Query("select count(*) from USER_ACCOUNT").Scalar();
            Assert.AreEqual(2, totalCount);

            var otherCount = sqler
                .Query("select count(*) from USER_ACCOUNT where username != @username")
                .FillParameters(parameters => parameters.AddWithValue("@username", "Jdoe"))
                .Scalar();
            Assert.AreEqual(1, otherCount);
        }

        [Test]
        public void TestReader()
        {
            var result = new List<UserAccount>(2);
            sqler.Query("select * from user_account").Reader(reader =>
            {
                while (reader.Read())
                {
                    result.Add(new UserAccount
                    {
                        Group = (string)reader[3],
                        Id = new Guid((string)reader[0])
                    });
                }
                reader.Close();
            });
            Assert.AreEqual("employees", result[1].Group);
        }
    }
}

namespace SimplaAdoNet.Entity
{
    public class UserAccount
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Group { get; set; }
    }
}
