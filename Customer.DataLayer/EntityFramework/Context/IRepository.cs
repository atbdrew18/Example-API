using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Customer.Api.DataLayer.EntityFramework.Entities;
using Customer.Api.DataLayer.EntityFramework.Context;

namespace Customer.Api.DataLayer.EntityFramework.Context
{
    /// <summary>
    /// Interface for Repository class (reading and maintaining data)
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IRepository : IDisposable
    {
        /// <summary>
        /// Get Data for the Specified Entity Type
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="includes">Related Items to Include</param>
        /// <returns>List of Entities</returns>
        IQueryable<T> GetAll<T>(params Expression<Func<T, object>>[] includes) where T : BaseEntity;

        /// <summary>
        /// Get Data for the Specified Entity Type with no Entity Framework DataContext tracking
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="includes">Related Items to Include</param>
        /// <returns></returns>
        IQueryable<T> GetAllAsUntracked<T>(params Expression<Func<T, object>>[] includes) where T : BaseEntity;

        /// <summary>
        /// Add Entity to the Database
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entity">Entity</param>
        void Add<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// Add all of the entities in the list to the Database.
        /// This will loop through each entity one by one, and call
        /// the Add method on the DBSet if the EntityState is Detached.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entities">Entity List</param>
        void AddAll<T>(IEnumerable<T> entities) where T : BaseEntity;

        /// <summary>
        /// Add a range of entities to the database.
        /// This will take the list of entities coming in, and call 
        /// the EF method AddRange on the entire list regardless of its state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        void AddRange<T>(IEnumerable<T> entities) where T : BaseEntity;

        /// <summary>
        /// Attach Entity to the Database Context
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entity">Entity</param>
        void Attach<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// Delete Specified Entity
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entity">Entity</param>
        void Delete<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// Delete all of the entities in the list from the Database
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entities">Entity List</param>
        void DeleteAll<T>(IEnumerable<T> entities) where T : BaseEntity;

        /// <summary>
        /// Commit Pending Changes
        /// </summary>
        void Commit();

        /// <summary>
        /// Set the command timeout for all contexts managed by the repository
        /// </summary>
        /// <param name="timeout"></param>
        void SetCommandTimeout(int timeout);

        /// <summary>
        /// Get the DB Contexts
        /// </summary>
        List<DataContext> GetDataContexts();

        /// <summary>
        /// Find a Record Base
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="id">Id</param>
        /// <returns>Entity</returns>
        T Find<T>(int id) where T : BaseEntity;

        /// <summary>
        /// Find a Record Base
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <typeparam name="I">Type of the Id Column</typeparam>
        /// <param name="id">Id</param>
        /// <returns>Entity</returns>
        T Find<T, I>(I id) where T : BaseEntity;

        /// <summary>
        /// Execute a Query Result using the Data Context
        /// </summary>
        /// <typeparam name="T">Type of Entity to Result</typeparam>
        /// <param name="request">Request Object</param>
        /// <returns>List of Entities</returns>
        IEnumerable<T> ExecuteList<T>(SQLRequest request) where T : BaseEntity;


        /// <summary>
        /// Execute a Query using the DataContext that supports paging.
        /// </summary>
        /// <typeparam name="T">Type of Entity to Result</typeparam>
        /// <param name="request">Request Object</param>
        /// <param name="rowCount">Rows</param>
        /// <returns>List of Entities</returns>
        IEnumerable<T> ExecuteListPaged<T>(SQLRequest request, out int rowCount) where T : BaseEntity;

        /// <summary>
        /// Returns and Begins a Transaction on the Default Data Context object
        /// </summary>
        /// <remarks>This method starts the Transaction on the database and should only be called from a using statement.</remarks>
        /// <returns></returns>
        IDbContextTransaction BeginTransaction();

        /// <summary>
        /// Returns and Begins a Transaction on the Default Data Context object
        /// </summary>
        /// <remarks>This method starts the Transaction on the database and should only be called from a using statement.</remarks>
        /// <returns></returns>
        IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Returns and Begins a Transaction on the specified Data Context object
        /// </summary>
        /// <remarks>This method starts the Transaction on the database and should only be called from a using statement.</remarks>
        /// <param name="contextName"></param>
        /// <returns></returns>
        IDbContextTransaction BeginTransaction(string contextName, IsolationLevel isolationLevel);

        /// <summary>
        /// Truncates the table backing the Entity type and returns the number of rows deleted.
        /// </summary>
        /// <typeparam name="T">Entity to truncate</typeparam>
        /// <returns></returns>
        void Truncate<T>() where T : BaseEntity;

        /// <summary>
        /// Undo all pending inserts/updates/deletes in the IRepository.
        /// </summary>
        void Rollback();

        int Execute(SQLRequest request);
    }
}
