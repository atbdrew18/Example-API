using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Customer.Api.DataLayer.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.EntityFramework.Context
{
    public class DataModelCacheKeyFactory : IModelCacheKeyFactory
    {

        public object Create(DbContext context)
            => Create(context, false);

        public object Create(DbContext context, bool designTime)
        {
            if (context is DataContext)
            {
                return new
                {
                    Type = context.GetType(),
                    (context as DataContext).CacheKey,
                    designTime
                };
            }
            else
            {
                return new
                {
                    Type = context.GetType(),
                    designTime
                };
            }
        }
    }
}
