using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.Base.ServiceModels
{
    public class BaseFetchListResponse<T> : BaseResponse
    {
        #region Properties
        /// <summary>
        /// Entities
        /// </summary>
        public List<T>? Entities { get; set; }

        /// <summary>
        /// DropDownList
        /// </summary>
        public List<BaseDropDownResponse>? DropDownList { get; set; }

        /// <summary>
        /// Total Count - used for pagination length
        /// </summary>
        public int TotalCount { get; set; }
        #endregion
    }
}
