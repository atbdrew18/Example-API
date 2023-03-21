using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.Base.ServiceModels
{
    public class BaseFetchRequest<T> : BaseRequest
    {
        #region Properties
        /// <summary>
        /// Primary Key for the entity
        /// </summary>
        public T Id { get; set; }
        #endregion
    }
}
