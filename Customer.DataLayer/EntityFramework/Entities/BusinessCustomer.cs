using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.EntityFramework.Entities
{
    [DataContextAttribute("DefaultConnection")]
    public class BusinessCustomer : BaseEntity
    {
        #region Properties
        /// <summary>
        /// BusinessCustomerId
        /// </summary>
        [Key]
        public int BusinessCustomerId { get; set; }
        /// <summary>
        /// CustomerId
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// BusinessId
        /// </summary>
        public int BusinessId { get; set; }

        /// <summary>
        /// Business
        /// </summary>
        public virtual Business? Business { get; set; }
        
        /// <summary>
        /// Customer
        /// </summary>
        public virtual Customer? Customer { get; set; }  
        #endregion
    }
}
