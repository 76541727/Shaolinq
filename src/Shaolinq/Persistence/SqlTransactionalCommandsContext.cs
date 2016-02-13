// Copyright (c) 2007-2015 Thong Nguyen (tumtumtum@gmail.com)

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Platform;
using Shaolinq.Persistence.Linq.Expressions;

namespace Shaolinq.Persistence
{
	public abstract partial class SqlTransactionalCommandsContext
		: IDisposable
	{
		internal MarsDataReader currentReader;

		private bool disposed;
		public bool SupportsAsync { get; protected set; }
		public IDbConnection DbConnection { get; private set; }
		public SqlDatabaseContext SqlDatabaseContext { get; }
		
		protected IDbTransaction dbTransaction;
		public DataAccessModel DataAccessModel { get; }
		private readonly bool emulateMultipleActiveResultSets;

		public abstract void Delete(SqlDeleteExpression deleteExpression);
		public abstract Task DeleteAsync(SqlDeleteExpression deleteExpression);
		public abstract Task DeleteAsync(SqlDeleteExpression deleteExpression, CancellationToken cancellationToken);
		public abstract void Delete(Type type, IEnumerable<DataAccessObject> dataAccessObjects);
		public abstract Task DeleteAsync(Type type, IEnumerable<DataAccessObject> dataAccessObjects);
		public abstract Task DeleteAsync(Type type, IEnumerable<DataAccessObject> dataAccessObjects, CancellationToken cancellationToken);

		public abstract void Update(Type type, IEnumerable<DataAccessObject> dataAccessObjects);
		public abstract Task UpdateAsync(Type type, IEnumerable<DataAccessObject> dataAccessObjects);
		public abstract Task UpdateAsync(Type type, IEnumerable<DataAccessObject> dataAccessObjects, CancellationToken cancellationToken);

		public abstract InsertResults Insert(Type type, IEnumerable<DataAccessObject> dataAccessObjects);
		public abstract Task<InsertResults> InsertAsync(Type type, IEnumerable<DataAccessObject> dataAccessObjects);
		public abstract Task<InsertResults> InsertAsync(Type type, IEnumerable<DataAccessObject> dataAccessObjects, CancellationToken cancellationToken);

		public abstract IDataReader ExecuteReader(string sql, IReadOnlyList<Tuple<Type, object>> parameters);
		public abstract Task<IDataReader> ExecuteReaderAsync(string sql, IReadOnlyList<Tuple<Type, object>> parameters);
		public abstract Task<IDataReader> ExecuteReaderAsync(string sql, IReadOnlyList<Tuple<Type, object>> parameters, CancellationToken cancellationToken);

		public virtual bool IsClosed => this.DbConnection.State == ConnectionState.Closed || this.DbConnection.State == ConnectionState.Broken;
		
        protected SqlTransactionalCommandsContext()
			: this(true)
		{
		}

        protected SqlTransactionalCommandsContext(bool supportsAsync)
		{
			this.SupportsAsync = supportsAsync;
		}

		public static System.Data.IsolationLevel ConvertIsolationLevel(DataAccessIsolationLevel isolationLevel)
		{
			switch (isolationLevel)
			{
			case DataAccessIsolationLevel.Serializable:
				return System.Data.IsolationLevel.Serializable;
			case DataAccessIsolationLevel.ReadCommitted:
				return System.Data.IsolationLevel.ReadCommitted;
			case DataAccessIsolationLevel.Chaos:
				return System.Data.IsolationLevel.Chaos;
			case DataAccessIsolationLevel.RepeatableRead:
				return System.Data.IsolationLevel.RepeatableRead;
			case DataAccessIsolationLevel.Snapshot:
				return System.Data.IsolationLevel.Snapshot;
			default:
				return System.Data.IsolationLevel.Unspecified;
			}
		}

		protected SqlTransactionalCommandsContext(SqlDatabaseContext sqlDatabaseContext, IDbConnection dbConnection, DataAccessTransaction transaction)
		{	
			this.DbConnection = dbConnection;
			this.SqlDatabaseContext = sqlDatabaseContext;
			this.DataAccessModel = sqlDatabaseContext.DataAccessModel;

			this.emulateMultipleActiveResultSets = !sqlDatabaseContext.SqlDialect.SupportsCapability(SqlCapability.MultipleActiveResultSets);

			this.dbTransaction = dbConnection.BeginTransaction(ConvertIsolationLevel(transaction?.IsolationLevel ?? DataAccessIsolationLevel.Unspecified));
		}

		~SqlTransactionalCommandsContext()
		{
			this.Dispose(false);
		}

		public virtual IDbCommand CreateCommand() => this.CreateCommand(SqlCreateCommandOptions.Default);

		public virtual IDbCommand CreateCommand(SqlCreateCommandOptions options)
		{
			var retval = this.DbConnection.CreateCommand();

			 retval.Transaction = this.dbTransaction;

			if (this.SqlDatabaseContext.CommandTimeout != null)
			{
				retval.CommandTimeout = (int)this.SqlDatabaseContext.CommandTimeout.Value.TotalMilliseconds;
			}

			if (this.emulateMultipleActiveResultSets)
			{
				retval = new MarsDbCommand(this, retval);
			}

			return retval;
		}

		public virtual void Prepare()
		{
			throw new NotSupportedException();
		}

		[RewriteAsync]
		public virtual void Commit()
		{
			try
			{
				this.dbTransaction.CommitEx();

				this.dbTransaction = null;
			}
			catch (Exception e)
			{
				var relatedSql = this.SqlDatabaseContext.GetRelatedSql(e);
				var decoratedException = this.SqlDatabaseContext.DecorateException(e, null, relatedSql);

				if (decoratedException != e)
				{
					throw decoratedException;
				}

				throw;
			}
			finally
			{
				this.CloseConnection();
			}
		}

		[RewriteAsync]
		public virtual void Rollback()
		{
			try
			{
				if (this.dbTransaction != null)
				{
					this.dbTransaction.RollbackEx();

					this.dbTransaction = null;
				}
			}
			finally
			{
				this.CloseConnection();
			}
		}

		[RewriteAsync]
		protected virtual void CloseConnection()
		{
			try
			{
				this.currentReader?.Dispose();
				this.currentReader = null;
			}
			catch
			{
			}

			try
			{
				this.dbTransaction.Dispose();
				this.dbTransaction = null;
			}
			catch
			{
			}

			this.DbConnection?.Close();
			this.DbConnection = null;
			
			GC.SuppressFinalize(this);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}

			ActionUtils.IgnoreExceptions(this.CloseConnection);

			this.disposed = true;
		}
	}
}
