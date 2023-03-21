using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.Base.ServiceModels
{
    public class BaseSaveResponse<T> : BaseResponse
    {
        #region Properties
        /// <summary>
        /// Entity
        /// </summary>
        public T? Entity { get; set; }
        #endregion
    }
}
