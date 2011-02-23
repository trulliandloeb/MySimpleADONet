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

        public EasySQLer Scalar(out object result)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                CheckAndSetCommand(conn);
                try
                {
                    conn.Open();
                    result = command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            Clear();
            return this;
        }

        public EasySQLer Reader(Action<SQLiteDataReader> readerAction, CommandBehavior behavior = CommandBehavior.Default)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                CheckAndSetCommand(conn);
                try
                {
                    conn.Open();
                    readerAction(command.ExecuteReader(behavior));
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            Clear();
            return this;
        }

        public EasySQLer Modify(out int number)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                CheckAndSetCommand(conn);
                conn.Open();
                var transaction = conn.BeginTransaction();
                command.Transaction = transaction;
                try
                {
                    number = command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
            Clear();
            return this;
        }

        private void CheckAndSetCommand(SQLiteConnection conn)
        {
            if (command == null)
            {
                command = new SQLiteCommand(sql);
            }
            command.Connection = conn;
        }

        private void Clear()
        {
            sql = null;
            command = null;
        }
    }
}
