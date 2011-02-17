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
            object totalCount, otherCount;
            sqler
                .Query("select count(*) from USER_ACCOUNT").Scalar(out totalCount)
                .Query("select count(*) from USER_ACCOUNT where username != @username")
                    .FillParameters(parameters => parameters.AddWithValue("@username", "Jdoe"))
                    .Scalar(out otherCount);
            Assert.AreEqual(2, totalCount);
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

        [Test]
        public void TestModify()
        {
            int insert, update, delete;
            sqler
                .Query("insert into user_account values(@id, @name, @password, @groupname)")
                    .FillParameters(parameters =>
                    {
                        parameters.AddWithValue("@id", "DA5816B0-E013-4AC3-918F-4F06C306BA22");
                        parameters.AddWithValue("@name", "sheldon");
                        parameters.AddWithValue("@password", "sheldor");
                        parameters.AddWithValue("@groupname", "doctor");
                    })
                    .Modify(out insert)
                .Query("update user_account set username = 'leonard' where id = 'DA5816B0-E013-4AC3-918F-4F06C306BA22'").Modify(out update)
                .Query("delete from user_account where id = 'DA5816B0-E013-4AC3-918F-4F06C306BA22'").Modify(out delete);

            Assert.AreEqual(1, insert);
            Assert.AreEqual(1, update);
            Assert.AreEqual(1, delete);
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
