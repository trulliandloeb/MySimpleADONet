using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SimpleAdoNet.Test
{
    [TestFixture]
    public class EasySQLerTest
    {
        private const string CONN_STRING = "data source=\"D:\\Program Files (x86)\\SQLite.NET\\db\\mysqlo\"";

        [Test]
        public void TestScalar()
        {
            var sqler = new EasySQLer(CONN_STRING);
            var totalCount = sqler.Query("select count(*) from USER_ACCOUNT").Scalar();
            Assert.AreEqual(2, totalCount);
        }
    }
}
