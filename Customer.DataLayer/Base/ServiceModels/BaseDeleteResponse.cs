using System;

namespace Customer.Api.DataLayer.Base.ServiceModels
{
    public class BaseDeleteResponse : BaseResponse
    {
        #region Properties
        /// <summary>
        /// The number of records that were deleted
        /// </summary>
        public int RecordsDeleted { get; set; }
        #endregion
    }
}