using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Data;

namespace SimpleAdoNet
{
    public class EasySQLer
    {
        private string connectionString;
        private string sql;
        private SQLiteCommand command;

        public EasySQLer(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public EasySQLer Query(string sql)
        {
            this.sql = sql;
            return this;
        }

        public EasySQLer FillParameters(Action<SQLiteParameterCollection> fillAction)
        {
            command = new SQLiteCommand(sql);
            fillAction(command.Parameters);
            return this;
        }

        public object Scalar()
        {
            object result = null;
            using (var conn = new SQLiteConnection(connectionString))
            {
                CheckCommand();
                command.Connection = conn;
                try
                {
                    conn.Open();
                    result = command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return result;
        }

        public void Reader(Action<SQLiteDataReader> readerAction, CommandBehavior behavior = CommandBehavior.Default)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                CheckCommand();
                command.Connection = conn;
                conn.Open();
                readerAction(command.ExecuteReader(behavior));
            }
        }

        private void CheckCommand()
        {
            if (command == null)
            {
                command = new SQLiteCommand(sql);
            }
        }
    }
}
