using System;
using System.Web;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Oracle.ManagedDataAccess.Client;

namespace utils
{
    public class DBConnection
    {
        public static DBConnection Instance => _instance;
        private static readonly DBConnection _instance = new DBConnection();

        private OracleConnection Connection;

        private DBConnection() { }

        public DBConnection SetConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) 
            {
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or empty.", nameof(connectionString));
            }

            Connection = new OracleConnection(connectionString);

            return this;
        }

        private void OpenConnection()
        {
            try
            {
                if (Connection?.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException($"{e.GetType()}, Error on OpenConnection!", nameof(e));
            }
        }

        private void CloseConnection()
        {
            try
            {
                if (Connection?.State != ConnectionState.Closed) 
                {
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException($"{e.GetType()}, Error on CloseConnection!", nameof(e));
            }
        }

        public T Query<T>(Func<OracleCommand, T> callback)
        {
            try
            {
                OpenConnection();
                OracleCommand command = Connection.CreateCommand();

                T result = callback(command);

                command.Dispose();
                CloseConnection();

                return result;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"{e.GetType()}, Error on Query!", nameof(e));
            }
        }

        public void Query(Action<OracleCommand> callback)
        {
            this.Query((command) =>
            {
                callback(command);
                return 0;
            });
        }

        public int NonQuery(string sql)
        {
            return this.Query((command) => {
                command.CommandText = sql.Trim();
                command.CommandType = CommandType.Text;

                command.Transaction = Connection.BeginTransaction();
                try
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    command.Transaction.Commit();

                    return rowsAffected;
                }
                catch (Exception)
                {
                    command.Transaction.Rollback();
                }

                return 0;
            });
        }

        public Hashtable[] Query(string sql)
        {
            return this.Query((command) => {
                command.CommandText = sql.Trim();
                command.CommandType = CommandType.Text;

                List<Hashtable> rows = new List<Hashtable>();
                OracleDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Hashtable row = new Hashtable();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.GetName(i), reader.IsDBNull(i) ? null : reader.GetValue(i));
                    }

                    rows.Add(row);
                }

                reader.Dispose();

                return rows.ToArray();
            });
        }
    }
}