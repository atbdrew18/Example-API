using Customer.Api.DataLayer.Domain.Customer;
using Customer.Api.DataLayer.Domain.Customer.ServiceModels;
using Customer.Api.DataLayer.EntityFramework.Context;
using Customer.Api.DataLayer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using ServiceModels = Customer.Api.DataLayer.Domain.Customer.ServiceModels;

namespace Customer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : BaseController
    {
        #region Private Members
        private readonly ICustomerService _customerService;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="customerService"></param>
        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService) : base(logger)
        {
            _customerService = customerService;
        }
        #endregion

        #region Actions
        /// <summary>
        /// Return a list of customers
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("fetch")]
        public virtual ActionResult Fetch(int id)
        {
            var response = _customerService.Fetch(new CustomerFetchRequest()
            {
                Id = id
            });
            return HandleResponse(response);
        }

        /// <summary>
        /// Return a list of customers
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("fetchlist")]
        public virtual ActionResult FetchList([FromBody] CustomerFetchListRequest request)
        {
            var response = _customerService.FetchList(request);
            return HandleResponse(response);
        }

        /// <summary>
        /// Save Customer
        /// </summary>
        /// <param name="saveRequest"></param>
        /// <returns></returns>
        [HttpPut, Route("Save")]
        public virtual ActionResult Save([FromBody] CustomerSaveRequest saveRequest) 
        {
            var response = _customerService.Save(saveRequest);
            return HandleResponse(response);
        }

        /// <summary>
        /// Delete Customer
        /// </summary>
        /// <param name="deleteRequest"></param>
        /// <returns></returns>
        [HttpDelete, Route("Delete")]
        public virtual ActionResult Delete([FromBody] CustomerDeleteRequest deleteRequest)
        {
            var response = _customerService.Delete(deleteRequest);
            return HandleResponse(response);
        }
        #endregion
    }
}