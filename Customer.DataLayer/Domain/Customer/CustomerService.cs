using Customer.Api.DataLayer.Base.Service;
using Customer.Api.DataLayer.Domain.Customer.ServiceModels;
using Customer.Api.DataLayer.EntityFramework.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities = Customer.Api.DataLayer.EntityFramework.Entities; 

namespace Customer.Api.DataLayer.Domain.Customer
{
    public class CustomerService : BaseEntityService, ICustomerService
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="repository"></param>
        public CustomerService(ILogger<CustomerService> logger, IRepository repository) : base(repository, logger)
        {
            
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Fetch a single Customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CustomerFetchResponse Fetch(CustomerFetchRequest request)
        {
            return Fetch<Entities.Customer, CustomerFetchResponse, int>(request);
        }

        /// <summary>
        /// Fetch a list of Customers
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CustomerFetchListResponse FetchList(CustomerFetchListRequest request)
        {
            return FetchList<Entities.Customer, CustomerFetchListResponse>(request);
        }

        /// <summary>
        /// Save a customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CustomerSaveResponse Save(CustomerSaveRequest request)
        {
            return Save<Entities.Customer, CustomerSaveResponse, int>(request);
        }

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CustomerDeleteResponse Delete(CustomerDeleteRequest request)
        {
            return Delete<Entities.Customer, CustomerDeleteResponse, int>(request);
        }
        #endregion
    }
}