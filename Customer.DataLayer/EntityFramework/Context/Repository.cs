using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Customer.Api.DataLayer.EntityFramework.Context.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using Customer.Api.DataLayer.EntityFramework.Entities;

namespace Customer.Api.DataLayer.EntityFramework.Context
{
    /// <summary>
    /// Provides methods for reading and maintaining data.
    /// </summary>
    /// <seealso cref="Catalyst.Core.EntityFramework.IRepository" />
    public class Repository : IRepository
    {

        #region Internals
        /// <summary>
        /// Dictionary of DbSets
        /// </summary>
        private readonly ConcurrentDictionary<Type, object> _dbSets = new ConcurrentDictionary<Type, object>();
        #endregion

        #region Properties
        /// <summary>
        /// List Of
        /// </summary>
        public List<DataContext> Contexts
        {
            get;
            private set;
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Repository()
        {
            Contexts = new List<DataContext>();
            InitialContexts();
        }
        #endregion

        #region Initial Contexts
        /// <summary>
        /// Initial Contexts
        /// </summary>
        protected void InitialContexts()
        {
            foreach (var item in DataContextConfig.Contexts)
            {
                var builder = new DbContextOptionsBuilder<DataContext>();

                builder.UseSqlServer(item.Value.ConnectionString, options => options.EnableRetryOnFailure()).ReplaceService<IModelCacheKeyFactory, DataModelCacheKeyFactory>();
                //NOTE: add this to log EF queries to console
                //builder.LogTo(Console.Write);
                var context = new DataContext(item.Value, builder.Options);
                
                Contexts.Add(context);
            }
        }
        #endregion

        #region Find / Get / Add / Delete
        /// <summary>
        /// Find a Record Base
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="id">Id</param>
        /// <returns>Entity</returns>
        public virtual T Find<T>(int id) where T : BaseEntity
        {
            var entity = GetDbSet<T>().Find(id);

            if (entity == null)
            {
                throw new EntityNotFoundException(id, (typeof(T)).Name);
            }

            return entity;
        }

        /// <summary>
        /// Find a Record at the Given ID
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="id">Id</param>
        /// <returns>Entity</returns>
        public virtual T Find<T, I>(I id) where T : BaseEntity
        {
            var entity = GetDbSet<T>().Find(id);

            if (entity == null)
            {
                throw new EntityNotFoundException(id, (typeof(T)).Name);
            }

            return entity;
        }


        /// <summary>
        /// Get Data for the Specified Entity Type
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="includes">Related Items to Include</param>
        /// <returns>List of Entities</returns>
        public virtual IQueryable<T> GetAll<T>(params Expression<Func<T, object>>[] includes) where T : BaseEntity
        {
            IQueryable<T> returnValue = GetDbSet<T>();

            if (includes != null)
            {
                returnValue = includes.Aggregate(returnValue, (current, include) => current.Include(include));
            }

            return returnValue;
        }

        /// <summary>
        /// Get Data for the Specified Entity Type with no Entity Framework DataContext tracking
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="includes">Related Items to Include</param>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllAsUntracked<T>(params Expression<Func<T, object>>[] includes) where T : BaseEntity
        {
            IQueryable<T> returnValue = GetDbSet<T>();

            if (includes != null)
            {
                returnValue = includes.Aggregate(returnValue, (current, include) => current.Include(include));
            }

            return returnValue.AsNoTracking();
        }

        /// <summary>
        /// Add Entity to the Database
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entity">Entity</param>
        public void Add<T>(T entity) where T : BaseEntity
        {
            DataContext context = GetContext(typeof(T));
            if (context.Entry(entity).State == EntityState.Detached)
            {
                GetDbSet<T>().Add(entity);
            }
        }

        /// <summary>
        /// Add all of the entities in the list to the Database.
        /// This will loop through each entity one by one, and call
        /// the Add method on the DBSet if the EntityState is Detached.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entities">Entity List</param>
        public void AddAll<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            if (entities == null || !entities.Any())
            {
                return;
            }
            DataContext context = GetContext(typeof(T));
            var list = entities.ToList();
            foreach (var entity in list)
            {
                if (context.Entry(entity) == null || context.Entry(entity).State == EntityState.Detached)
                {
                    GetDbSet<T>().Add(entity);
                }
            }
        }

        /// <summary>
        /// Add a range of entities to the database.
        /// This will take the list of entities coming in, and call
        /// the EF method AddRange on the entire list regardless of its state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public void AddRange<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            if (entities == null || !entities.Any())
            {
                return;
            }
            DataContext context = GetContext(typeof(T));
            GetDbSet<T>().AddRange(entities);
        }

        /// <summary>
        /// Attach Entity to the Database Context
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entity">Entity</param>
        public void Attach<T>(T entity) where T : BaseEntity
        {
            DataContext context = GetContext(typeof(T));
            if (context.Entry(entity).State == EntityState.Detached)
            {
                GetDbSet<T>().Attach(entity);
            }
        }

        /// <summary>
        /// Delete Specified Entity
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entity">Entity</param>
        public void Delete<T>(T entity) where T : BaseEntity
        {
            GetDbSet<T>().Remove(entity);
        }

        /// <summary>
        /// Delete all of the entities in the list from the Database
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entities">Entity List</param>
        public void DeleteAll<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            if (entities == null || !entities.Any())
            {
                return;
            }
            var list = entities.ToList();
            foreach (var entity in list)
            {
                GetDbSet<T>().Remove(entity);
            }
        }
        #endregion

        #region Commit

        /// <summary>
        /// Commit Pending Changes
        /// </summary>
        public virtual void Commit()
        {
            foreach (var context in this.Contexts)
            {
                if (context != null && context.ChangeTracker != null && context.ChangeTracker.Entries() != null && context.ChangeTracker.Entries().Any(HasChanged))
                {
                    foreach (var dbEntityEntry in context.ChangeTracker.Entries<BaseEntity>())
                    {
                        // Otherwise, lookup the Type in the AuditInfo cache and set via the Stamp properties
                        Type entityType = dbEntityEntry.Entity.GetType();

                        // If the type is a proxy type, get the base type to see the real entity type
                        if (entityType.FullName.StartsWith("System.Data.Entity.DynamicProxies."))
                        {
                            entityType = entityType.BaseType;
                        }
                    }

                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Has Changes
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Pending Changes</returns>
        private static bool HasChanged(EntityEntry entity)
        {
            if (entity == null)
            {
                return false;
            }
            return IsState(entity, EntityState.Added) ||
                   IsState(entity, EntityState.Deleted) ||
                   IsState(entity, EntityState.Modified);
        }

        /// <summary>
        /// Check the state
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="state">State</param>
        /// <returns>State matches the entity state</returns>
        private static bool IsState(EntityEntry entity, EntityState state)
        {
            return (entity.State & state) == state;
        }
        #endregion

        #region Set the Command Timeout
        /// <summary>
        /// Set the command timeout on all of the DB contexts that the repository is managing.
        /// The timeout value is specified in seconds.
        /// </summary>
        /// <param name="timeout">Timeout value in seconds</param>
        public void SetCommandTimeout(int timeout)
        {
            foreach (var context in this.Contexts)
            {
                context.Database.SetCommandTimeout(timeout);
            }
        }
        #endregion

        #region Get the Data Contexts
        /// <summary>
        /// Get the Data Contexts that the repository is managing.
        /// </summary>
        public List<DataContext> GetDataContexts()
        {
            return this.Contexts;
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose inner contexts
        /// </summary>
        /// <param name="disposing">Disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Contexts != null)
                {
                    for (int i = this.Contexts.Count; i > 0; i--)
                    {
                        this.Contexts[i - 1].Dispose();
                        this.Contexts.RemoveAt(i - 1);
                    }
                }
            }
        }
        #endregion

        #region Get Context / DbSet
        /// <summary>
        /// Get the Context for the provided type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>DataContext</returns>
        protected DataContext GetContext(Type t)
        {
            DataContext context = this.Contexts.FirstOrDefault(c => c.Entities.ContainsKey(t));

            if (context == null)
            {
                if (this.Contexts == null || this.Contexts.Count == 0)
                {
                    throw new Exception(string.Format("Could not find entity {0} in application context. Context list is empty.", t.ToString()));
                }
                else
                {
                    string contextList = String.Join("|", this.Contexts.Select(x => x.CacheKey + ":" + String.Join("|", x.Entities.Select(n => n.ToString()).ToArray())).ToArray());
                    throw new Exception(string.Format("Could not find entity {0} in application contexts({1}). Entity list: {2}", t.ToString(), this.Contexts.Count, contextList));
                }
            }
            return context;
        }

        /// <summary>
        /// Get the DbSet for the provided Type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>DbSet</returns>
        private DbSet<T> GetDbSet<T>() where T : class
        {
            DataContext context = GetContext(typeof(T));

            DbSet<T> set = null;
            try
            {
                set = (DbSet<T>)_dbSets.GetOrAdd(typeof(T), x => context.Set<T>());
            }
            catch (Exception ex)
            {
                Exception e = new Exception(string.Format("Issue with DbSet GetOrAdd for entity {0}. The current context is {1}", typeof(T).ToString(), context.CacheKey), ex);
                throw e;
            }
            return set;
        }
        #endregion

        #region Execute Methods
        public virtual int Execute(SQLRequest request)
        {
            var defaultContext = this.Contexts.FirstOrDefault();

            if (defaultContext == null)
            {
                return -1;
            }

            var result = defaultContext.Database.ExecuteSqlRaw(request.SqlCommand);

            return result;
        }

        /// <summary>
        /// Execute a Query Result using the Data Context
        /// </summary>
        /// <typeparam name="T">Type of Entity to Result</typeparam>
        /// <param name="request">Request Object</param>
        /// <returns>List of Entities</returns>
        public virtual IEnumerable<T> ExecuteList<T>(SQLRequest request) where T : BaseEntity
        {
            var dbSet = GetDbSet<T>();

            var result = dbSet.FromSqlRaw<T>(request.SqlCommand, request.GetParameters());

            return result.ToList();
        }

        /// <summary>
        /// Execute a Query Result using the Data Context, and expect a second result set containing the row count
        /// </summary>
        /// <typeparam name="T">Type of Entity to Result</typeparam>
        /// <param name="request">Request Object</param>
        /// <param name="rowCount">Number of rows</param>
        /// <returns>List of Entities</returns>
        public virtual IEnumerable<T> ExecuteListPaged<T>(SQLRequest request, out int rowCount) where T : BaseEntity
        {
            DataContext context = GetContext(typeof(T));
            IEnumerable<T> result;

            using (var connection = context.Database.GetDbConnection())
            {
                using (System.Data.Common.DbCommand command = connection.CreateCommand())
                {
                    // Setup command
                    command.CommandText = request.SqlCommand;
                    object[] sqlParams = request.GetParameters();
                    if (sqlParams != null && sqlParams.Any())
                    {
                        command.Parameters.AddRange(sqlParams);
                    }

                    // Check connection
                    bool closeConnectionWhenFinished = false;
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Open();
                        closeConnectionWhenFinished = true;
                    }

                    // Retrieve each result set in turn
                    using (System.Data.Common.DbDataReader reader = command.ExecuteReader())
                    {
                        result = ReadAll<T>(reader);

                        reader.NextResult();
                        if (reader.HasRows == false)
                        {
                            throw new Exception("The SQLRequest did not return the row count as a second result set. " + request.SqlCommand);
                        }

                        reader.Read();
                        rowCount = int.Parse(reader[0].ToString());
                    }

                    // Re-close connection if necessary
                    if (closeConnectionWhenFinished)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            return result;
        }

        #endregion


        #region Transaction Methods
        /// <summary>
        /// Returns and Begins a Transaction on the specified Data Context object
        /// </summary>
        /// <remarks>This method starts the Transaction on the database and should only be called from a using statement.</remarks>
        /// <param name="contextName"></param>
        /// <returns></returns>
        public virtual IDbContextTransaction BeginTransaction(string contextName, IsolationLevel isolationLevel)
        {
            if (string.IsNullOrWhiteSpace(contextName))
            {
                throw new ArgumentNullException("Parameter 'contextName' must have a non-empty value.");
            }

            DataContext context = GetDataContexts().Find(ctx => ctx.CacheKey.Equals(contextName));
            if (context == null)
            {
                throw new ArgumentException("Failed to find DataContext with name: " + contextName);
            }

            return context.Database.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Returns and Begins a Transaction on the Default Data Context object
        /// </summary>
        /// <remarks>This method starts the Transaction on the database and should only be called from a using statement.</remarks>
        /// <returns></returns>
        public virtual IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return BeginTransaction("DefaultConnection", isolationLevel);
        }

        /// <summary>
        /// Returns and Begins a Transaction on the Default Data Context object
        /// </summary>
        /// <remarks>This method starts the Transaction on the database and should only be called from a using statement.</remarks>
        /// <returns></returns>
        public virtual IDbContextTransaction BeginTransaction()
        {
            return BeginTransaction("DefaultConnection", IsolationLevel.ReadCommitted);
        }
        #endregion

        #region Truncate Table
        /// <summary>
        /// Truncates the table backing the Entity type and returns the number of rows deleted.
        /// </summary>
        /// <typeparam name="T">Entity to truncate</typeparam>
        /// <returns></returns>
        public void Truncate<T>() where T : BaseEntity
        {
            var dataContext = GetContext(typeof(T));

            var mapping = dataContext.Model.FindEntityType(typeof(T));
            var schema = mapping.GetSchema();
            var tableName = mapping.GetTableName();

            dataContext.Database.ExecuteSqlRaw(String.Format("TRUNCATE TABLE {0}.{1}", schema, tableName));
        }
        #endregion

        #region Rollback
        /// <summary>
        /// Undo all pending inserts/updates/deletes in the IRepository.
        /// </summary>
        public virtual void Rollback()
        {
            var contexts = this.GetDataContexts();
            foreach (var context in contexts)
            {
                Rollback(context);
            }
        }

        /// <summary>
        /// Undo all pending inserts/updates/deletes in the DataContext.
        /// </summary>
        /// <param name="dataContext">The DataContext to rollback.</param>
        protected virtual void Rollback(DataContext dataContext)
        {
            var changedEntries = dataContext.ChangeTracker
                .Entries()
                .Where(x => x.State != EntityState.Unchanged)
                .ToList();

            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;

                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }
        #endregion

        protected List<TEntity> ReadAll<TEntity>(DbDataReader reader)
            where TEntity : BaseEntity
        {
            var rows = Activator.CreateInstance<List<TEntity>>();
            var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            while (reader.Read())
            {
                var row = Activator.CreateInstance<TEntity>();
                foreach (var column in columns)
                {
                    var value = reader[column] == DBNull.Value ? null : reader[column];
                    row.GetType().GetProperty(column).SetValue(row, value);
                }
                rows.Add(row);
            }
            return rows;
        }
    }
}
