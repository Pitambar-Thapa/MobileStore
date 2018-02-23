using StoreRepository.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;

namespace StoreRepository.Repositories
{
    /// <summary>
    /// UnitOfWork class of type IUnitOfWork
    /// </summary>
    /// <summary>
    /// UnitOfWork class of type IUnitOfWork
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Transaction object
        /// </summary>
        private DbTransaction transaction;

        /// <summary>
        /// Status of disposal
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the class UnitOfWork
        /// </summary>
        public UnitOfWork()
        {
            try
            {
                foreach (System.Configuration.ConnectionStringSettings con in System.Configuration.ConfigurationManager.ConnectionStrings)
                {
                    if (con.Name.ToLower(CultureInfo.InvariantCulture).Contains("entities"))
                    {
                        this.DBContext = new DbContext(con.ConnectionString);
                        break;
                    }
                }

                if (this.DBContext == null)
                {
                    throw new EntityException("Context not initialized");
                }
            }
            catch
            {
                throw new EntityException("Context not initialized");
            }
        }

        /// <summary>
        /// Initializes a new instance of the class UnitOfWork
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        public UnitOfWork(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString", "Connection string not correct");
            }

            this.DBContext = new DbContext(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the class UnitOfWork
        /// </summary>
        /// <param name="context">Database context</param>
        public UnitOfWork(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.DBContext = context;
        }

        /// <summary>
        /// Property for DbContext
        /// </summary>
        public DbContext DBContext
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets if transaction is active.
        /// </summary>
        public bool IsInTransaction
        {
            get
            {
                return this.transaction != null;
            }
        }

        Context IUnitOfWork.DBContext
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Saves changes.
        /// </summary>
        public void SaveChanges()
        {
            if (this.IsInTransaction)
            {
                throw new ApplicationException("A transaction is active. Use CommitTransaction to save changes.");
            }

            ((IObjectContextAdapter)this.DBContext).ObjectContext.SaveChanges();
        }

        /// <summary>
        /// Saves changes as per specified options.
        /// </summary>
        /// <param name="saveOptions">Save options.</param>





        /// <summary>
        /// Starts a transaction.
        /// </summary>
        public void BeginTransaction()
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Starts a transaction with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">isolation level</param>
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (this.transaction != null)
            {
                throw new ApplicationException("A transaction is already active. Commit or rollback the existing transaction before starting a new one.");
            }

            // Opens a new connection
            this.OpenConnection();

            // Start a new transaction with the specified isolation level.
            this.transaction = ((IObjectContextAdapter)this.DBContext).ObjectContext.Connection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Rolls back a transaction.
        /// </summary>
        public void RollbackTransaction()
        {
            this.transaction.Rollback();
        }

        /// <summary>
        /// Commits a transaction.
        /// </summary>
        public void CommitTransaction()
        {
            if (this.transaction == null)
            {
                throw new ApplicationException("Cannot rollback a transaction. There is no active transaction.");
            }

            try
            {
                ////Saves context changes and commit transaction.
                ((IObjectContextAdapter)this.DBContext).ObjectContext.SaveChanges();
                this.transaction.Commit();
            }
            catch
            {
                ////Discards changes and rolls back transaction.
                this.transaction.Rollback();
                throw;
            }
            finally
            {
                this.ReleaseCurrentTransaction();
            }
        }

        /// <summary>
        /// Disposes (releases resources).
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Opens a connection.
        /// </summary>
        private void OpenConnection()
        {
            if (((IObjectContextAdapter)this.DBContext).ObjectContext.Connection.State != ConnectionState.Open)
            {
                ((IObjectContextAdapter)this.DBContext).ObjectContext.Connection.Open();
            }
        }

        /// <summary>
        /// Releases the current transaction.
        /// </summary>
        private void ReleaseCurrentTransaction()
        {
            if (this.transaction != null)
            {
                this.transaction.Dispose();
                this.transaction = null;
            }
        }

        /// <summary>
        /// Dispose process.
        /// </summary>
        /// <param name="disposing">if class disposing</param>
        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
        }
    }
}
