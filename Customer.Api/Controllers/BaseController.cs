using Customer.Api.DataLayer.Base.ServiceModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        #region Properties
        public readonly ILogger _logger;
        //public readonly IHttpContextAccessor _httpContext;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        //public BaseController(ILogger logger, IHttpContextAccessor httpContext)
        public BaseController(ILogger logger)
        {
            _logger = logger;
            //_httpContext = httpContext;
        }
        #endregion

        /// <summary>
        /// Handle the response for the correct return type
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected internal ActionResult HandleResponse(BaseResponse response)
        {
            try
            {
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex) 
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
