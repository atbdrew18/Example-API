using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.EntityFramework.Entities
{
    /// <summary>
    /// Base Entity
    /// </summary>
    [Serializable]
    public class BaseEntity : BaseModel
    {
        #region Properties
        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// CreateUser
        /// </summary>
        public string? CreateUser { get; set; }

        /// <summary>
        /// ModifyDate
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// ModifyUser
        /// </summary>
        public string? ModifyUser { get; set; }
        #endregion
    }
}
