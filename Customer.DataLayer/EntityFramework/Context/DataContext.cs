using Microsoft.EntityFrameworkCore;
using Customer.Api.DataLayer.EntityFramework.Entities;
using Customer.Api.DataLayer.Helpers.Encryption;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Customer.Api.DataLayer.EntityFramework.Context
{
    public class DataContext : DbContext
    {
        #region Internals
        /// <summary>
        /// Connection Name
        /// </summary>
        private string _connectionName = "";

        private DataContextConfig _setup;

        /// <summary>
        /// Dictionary of DbSets
        /// </summary>
        private readonly ConcurrentDictionary<Type, object> _dbSets = new ConcurrentDictionary<Type, object>();
        #endregion

        #region Properties
        /// <summary>
        /// Entity List
        /// </summary>
        public Dictionary<Type, Type> Entities
        {
            get
            {
                return DataContextConfig.Contexts[_connectionName].Entities;
            }
        }

        /// <summary>
        /// Cache Key Name / Used to Implement IDbModelCacheKeyProvider ( Provides the ability for DataContext to be Multi-Tentant)
        /// </summary>
        public string CacheKey
        {
            get
            {
                return _connectionName;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="setup">Data content configuration</param>
        public DataContext(DataContextConfig setup, DbContextOptions options)
            : base(options)
        {
            _setup = setup;
            _connectionName = setup.Name;
        }
        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var applyEntityConfigurationMethod = typeof(ModelBuilder)
                .GetMethods()
                .Single(
                    e => e.Name == "ApplyConfiguration"
                        && e.ContainsGenericParameters
                        && e.GetParameters().SingleOrDefault()?.ParameterType.GetGenericTypeDefinition()
                        == typeof(IEntityTypeConfiguration<>));

            foreach (var item in this.Entities)
            {
                if (item.Value == null)
                {
                    modelBuilder.Entity(item.Key);
                }
            }
            foreach (var item in this.Entities)
            {
                if (item.Value != null)
                {
                    var type = item.Value;

                    foreach (var @interface in type.GetInterfaces())
                    {
                        if (!@interface.IsGenericType)
                        {
                            continue;
                        }

                        if (@interface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                        {
                            var target = applyEntityConfigurationMethod.MakeGenericMethod(@interface.GenericTypeArguments[0]);
                            target.Invoke(modelBuilder, new[] { Activator.CreateInstance(type) });
                        }
                    }
                }
            }

            modelBuilder.UseEncryption();

            base.OnModelCreating(modelBuilder);
        }

        #region Borrowed from IRepository
        /// <summary>
        /// Get the DbSet for the provided Type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>DbSet</returns>
        private DbSet<T> GetDbSet<T>() where T : class
        {
            DataContext context = this;

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

        /// <summary>
        /// Get Data for the Specified Entity Type
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="includes">Related Items to Include</param>
        /// <returns>List of Entities</returns>
        public IQueryable<T> GetAll<T>(params Expression<Func<T, object>>[] includes) where T : BaseEntity
        {
            IQueryable<T> returnValue = GetDbSet<T>();

            if (includes != null)
            {
                returnValue = includes.Aggregate(returnValue, (current, include) => current.Include(include));
            }

            return returnValue;
        }

        /// <summary>
        /// Add Entity to the Database
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entity">Entity</param>
        public void Add<T>(T entity) where T : BaseEntity
        {
            DataContext context = this;
            if (context.Entry(entity).State == EntityState.Detached)
            {
                GetDbSet<T>().Add(entity);
            }
        }
        #endregion
    }
}
