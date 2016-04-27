using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// Provides additional options for creating a transaction scope
    /// </summary>
    public enum TransactionScopeOption
    {
        /// <summary>
        ///A transaction is required by the scope.It uses an ambient transaction if
        ///one already exists.Otherwise, it creates a new transaction before entering
        ///the scope.This is the default value.  
        /// </summary>
        Required = 0,
        /// <summary>
        /// A new transaction is always created for the scope.
        /// </summary>
        RequiresNew = 1,

        /// <summary>
        ///The ambient transaction context is suppressed when creating the scope.All
        ///operations within the scope are done without an ambient transaction context.        
        /// </summary>
        Suppress = 2,
    }
    /// <summary>
    /// Specifies the isolation level of a transaction.
    /// </summary>
    public enum IsolationLevel
    {
        /// <summary>
        /// Volatile data can be read but not modified, and no new data can be added
        /// during the transaction.        
        /// </summary>
        Serializable = 0,
        /// <summary>
        /// Volatile data can be read but not modified during the transaction.New data
        /// can be added during the transaction.        
        /// </summary>
        RepeatableRead = 1,
        /// <summary>
        /// Volatile data cannot be read during the transaction, but can be modified.
        /// </summary>
        ReadCommitted = 2,
        /// <summary>
        /// Volatile data can be read and modified during the transaction.
        /// </summary>
        ReadUncommitted = 3,
        /// <summary>
        /// Volatile data can be read.Before a transaction modifies data, it verifies
        /// if another transaction has changed the data after it was initially read.If
        /// the data has been updated, an error is raised.This allows a transaction to
        /// get to the previously committed value of the data.    
        /// </summary>
        Snapshot = 4,
        /// <summary>
        /// The pending changes from more highly isolated transactions cannot be overwritten.
        /// </summary>
        Chaos = 5,
        /// <summary>
        ///A different isolation level than the one specified is being used, but the
        ///level cannot be determined.An exception is thrown if this value is set.
        /// </summary>
        Unspecified = 6,
    }
    /// <summary>
    /// 等价于System.Transactions.TransactionOptions
    /// </summary>
    public class TransactionOptions
    {
        /// <summary>
        /// Gets or sets the isolation level.
        /// </summary>
        /// <value>
        /// The isolation level.
        /// </value>
        public IsolationLevel IsolationLevel { get; set; }
        /// <summary>
        /// A System.TimeSpan value that specifies the timeout period for the transaction
        /// </summary>
        public TimeSpan Timeout { get; set; }
    }


}
