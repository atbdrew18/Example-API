using Customer.Api.DataLayer.Domain.Customer.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities = Customer.Api.DataLayer.EntityFramework.Entities;

namespace Customer.Api.DataLayer.Domain.Customer
{
    public interface ICustomerService
    {
        CustomerFetchResponse Fetch(CustomerFetchRequest request);
        CustomerFetchListResponse FetchList(CustomerFetchListRequest request);
        CustomerSaveResponse Save(CustomerSaveRequest request);
        CustomerDeleteResponse Delete(CustomerDeleteRequest request);
    }
}
