using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace SimpleAdoNet
{
    public class EasySQLer<TConnection, TCommand>
        where TConnection : DbConnection, new()
        where TCommand : DbCommand, new()
    {
        private string connectionString;
        private string sql;
        private TCommand command;

        public EasySQLer(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public EasySQLer<TConnection, TCommand> Query(string sql)
        {
            this.sql = sql;
            return this;
        }

        public EasySQLer<TConnection, TCommand> FillParameters(Action<DbParameterCollection> fillAction)
        {
            command = new TCommand();
            command.CommandText = sql;
            fillAction(command.Parameters);
            return this;
        }

        public EasySQLer<TConnection, TCommand> Scalar(out object result)
        {
            using (var conn = new TConnection())
            {
                conn.ConnectionString = connectionString;
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

        public EasySQLer<TConnection, TCommand> Reader(Action<DbDataReader> readerAction, CommandBehavior behavior = CommandBehavior.Default)
        {
            using (var conn = new TConnection())
            {
                conn.ConnectionString = connectionString;
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

        public EasySQLer<TConnection, TCommand> Modify(out int number)
        {
            using (var conn = new TConnection())
            {
                conn.ConnectionString = connectionString;
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

        private void CheckAndSetCommand(TConnection conn)
        {
            if (command == null)
            {
                command = new TCommand();
                command.CommandText = sql;
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
