using System.Collections.Generic;

namespace Customer.Api.DataLayer.Base.ServiceModels
{
    public class BaseRequest
    {
        #region Properties
        /// <summary>
        /// RequestingUserEmailAddress
        /// </summary>
        public string? RequestingUserEmailAddress { get; set; }

        /// <summary>
        /// RequestingUserADUserId
        /// </summary>
        public string? RequestingUserADUserId { get; set; }

        /// <summary>
        /// RequestingUserRoles
        /// </summary>
        public List<string>? RequestingUserRoles { get; set; }
        #endregion
    }
}
