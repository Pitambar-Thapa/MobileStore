using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace StoreRepository.Infrastructure
{
    /// <summary>
    /// Unit of work interface for transactions
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets if transaction is active.
        /// </summary>
        bool IsInTransaction { get; }

        /// <summary>
        /// Gets current database context
        /// </summary>
        Context DBContext { get; }

        /// <summary>
        /// Saves changes.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Saves changes as per specified options.
        /// </summary>
        /// <param name="saveOptions">Save options.</param>



        /// <summary>
        /// Starts a transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Starts a transaction with isolation level.
        /// </summary>
        /// <param name="isolationLevel">isolation level</param>
        void BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Rolls back a transaction.
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Commits a transaction.
        /// </summary>
        void CommitTransaction();
    }
}
