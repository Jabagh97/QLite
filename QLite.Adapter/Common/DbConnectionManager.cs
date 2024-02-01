using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NPoco;
using Quavis.Kapp.Data.Dto.ErrorHandling;
using QLite.Common;
using QLite.Data.Dto.Kapp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace QLite.Data.Adapter.Common
{
    public interface IDbConnectionManager : IDisposable
    {
        DbConnection Connection { get; }

        IDatabase Db { get; }

        bool OpenConnection();
        bool CloseConnection();

        TransactionScope BeginTransactionScope(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);
        //IDbTransaction BeginTransaction();
    }

    /// <summary>
    /// SqlConnection'un üretildiği ve dağıtıldığı aynı zamanda istendiğinde transaction'u da üreten class.
    /// Proje için "per request" olacak şekilde dataadapterlere "inject" edilmeli.
    /// </summary>
    public class DbConnectionManager : IDbConnectionManager
    {
        IConfiguration _cfg;
        public DbConnectionManager(IConfiguration cfg)
        {
            _cfg = cfg;
        }


        private DbConnection connection;

        /// <summary>
        /// Tüm "DataAdapter" nesnelerinde ortak kullanılan "connection" nesnesi
        /// </summary>
        public DbConnection Connection
        {
            get
            {
                if (connection == null)
                {                    
                    string connectionString = _cfg.GetValue<string>("ConnectionStrings:ConnectionString");

                    if (!connectionString.Contains("Data Source="))
                    {
                        var fn = Path.GetFileName(connectionString);
                        var filePath = Path.Combine(AppContext.BaseDirectory, fn);
                        connection = new SqliteConnection($"Data Source={filePath}");
                    }
                    else
                        connection = new SqliteConnection(connectionString);

                }
                return connection;
            }
        }

        /// <summary>
        /// Tüm "DataAdapter" nesnelerinde ortak kullanılan "transaction" nesnesi
        /// </summary>
        public DbTransaction Transaction { get; private set; }

        IDatabase _Db;
        public IDatabase Db
        {
            get
            {
                OpenConnection();
                if (_Db == null)
                {
                    _Db = new Database(Connection, DatabaseType.SQLite);
                }

                return _Db;
            }
        }

        private string connectionOwnerMethodName;
        private string transactionOwnerMethodName;
        private readonly HashSet<string> authorizedClassNames = new()
        {
            "QLite.Server.Middlewares.ExceptionHandlerMiddleware"
        };


        public bool OpenConnection()
        {
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    connectionOwnerMethodName = GetCallerMethodFullName().fullName;
                    Connection.Open();

                    // Sadece 'select' çalışacak requestlerin Sql'de lock'a takılmaması için
                    Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted).Commit();
                }
                catch (Exception ex)
                {
                    LoggerAdapter.Error(ex, "dbConnMan opening conn");
                    throw;
                }
            }
            return Connection.State == ConnectionState.Open;
        }

        public bool CloseConnection()
        {
            //if (connection.State != ConnectionState.Closed && IsCallerMethodAuthorized(AuthorizeType.Connection))
            if (connection != null && connection.State != ConnectionState.Closed)
            {
                try
                {
                    connection.Close();
                    var sqlConnection = (connection as SqlConnection);
                    string connectionId = sqlConnection != null ? $" Connection Id: {sqlConnection.ClientConnectionId}" : "";
                }
                catch (Exception ex)
                {
                    LoggerAdapter.Error(ex,"Closing Conn");

                    throw;
                }
            }
            return Connection.State == ConnectionState.Closed;
        }

        private bool IsCallerMethodAuthorized(AuthorizeType type)
        {
            var (className, fullName) = GetCallerMethodFullName();
            if (authorizedClassNames.Contains(className))
            {
                return true;
            }

            return type switch
            {
                AuthorizeType.Connection => String.Equals(fullName, connectionOwnerMethodName),
                AuthorizeType.Transaction => String.Equals(fullName, transactionOwnerMethodName),
                _ => throw new ArgumentException("DbConnectionManager.AuthorizeType cannot be defined.", nameof(type)),
            };
        }

        private (string className, string fullName) GetCallerMethodFullName()
        {
            var stack = new StackTrace();
            foreach (var frame in stack.GetFrames())
            {
                var method = frame.GetMethod();

                // DBConnectionManager dışındaki metodu arıyoruz
                if (method.DeclaringType != GetType())
                {
                    string className = method.DeclaringType.FullName.Split('+')[0];
                    string fullName = $"{className}.{method.Name}";
                    return (className, fullName);
                }
            }

            // Hani olmazda... nolur nolmaz
            throw KappException.NewKappException("DBConnectionManager çağıran metod ismini bulamadı!", KappErrorCodes.DbConnManError);
        }


        public TransactionScope BeginTransactionScope(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            if (OpenConnection())
            {

                var scope = new TransactionScope(TransactionScopeOption.Required,
                                new TransactionOptions()
                                {
                                    IsolationLevel = isolationLevel
                                });
                //Connection.EnlistTransaction(System.Transactions.Transaction.Current);
                return scope;
            }
            throw new InvalidOperationException("Connection cannot be opened.");
        }

        //public IDbTransaction BeginTransaction()
        //{
        //    if (OpenConnection())
        //    {
        //        return Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        //    }
        //    throw new InvalidOperationException("Connection cannot be opened.");
        //}
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_Db != null)
                {
                    _Db.Dispose();
                    _Db = null;
                }
                if (connection != null)
                {
                    CloseConnection();
                    connection.Dispose();
                    connection = null;
                }
            }
        }

        private enum AuthorizeType
        {
            Connection,
            Transaction
        }
    }
}
