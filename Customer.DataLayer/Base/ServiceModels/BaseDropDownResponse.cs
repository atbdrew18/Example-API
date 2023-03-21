using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.Base.ServiceModels
{
    public class BaseDropDownResponse
    {
        #region Property
        /// <summary>
        /// Key
        /// </summary>
        public int Key { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string? Value { get; set; }
        #endregion

    }
}
