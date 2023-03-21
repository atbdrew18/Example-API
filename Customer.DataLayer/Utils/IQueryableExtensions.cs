using Customer.Api.DataLayer.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.Utils
{
    public static class IQueryableExtensions
    {
        #region Public Methods
        /// <summary>
        /// Order by Asc
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByAsc<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }

        /// <summary>
        /// Order by Desc
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        /// <summary>
        /// Get Value for property of generic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturnType"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static TReturnType? GetValueFromGeneric<T, TReturnType>(T obj, string propName)
            where T : BaseEntity
        {
            return (TReturnType?)Convert.ChangeType(obj.GetType().GetProperty(propName).GetValue(obj, null), typeof(TReturnType?));
        }

        /// <summary>
        /// Create a slect statement dynamically
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Func<TEntity, TEntity> CreateDynamicSelectStatement<TEntity>(params string[] fields)
            where TEntity : BaseEntity
        {
            var xParameter = Expression.Parameter(typeof(TEntity), "o");
            var xNew = Expression.New(typeof(TEntity));
            var bindings = fields.Select(o => o.Trim())
                .Select(fieldName =>
                {
                    var propertyInfo = typeof(TEntity).GetProperty(fieldName);
                    if (propertyInfo == null)
                    {
                        throw new Exception($"{fieldName} doesn't exist. Please check if the field exists or if the spelling is correct");
                    }

                    var memberExpression = Expression.Property(xParameter, propertyInfo);
                    return Expression.Bind(propertyInfo, memberExpression);
                }
            );

            var xInit = Expression.MemberInit(xNew, bindings);
            var lambda = Expression.Lambda<Func<TEntity, TEntity>>(xInit, xParameter);

            // compile to Func<Data, Data>
            return lambda.Compile();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// To Lambda
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }
        #endregion

    }
}
