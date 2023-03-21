using System;

namespace Customer.Api.DataLayer.Base.ServiceModels
{
    public class BaseDeleteRequest<T> : BaseRequest
    {
        #region Properties
        /// <summary>
        /// The list of IDs to delete
        /// </summary>
        public IEnumerable<T>? Ids { get; set; }
        #endregion
    }
}