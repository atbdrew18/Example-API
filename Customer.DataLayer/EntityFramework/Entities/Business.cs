using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.EntityFramework.Entities
{
    [DataContextAttribute("DefaultConnection")]
    public class Business : BaseEntity
    {
        /// <summary>
        /// BusinessId
        /// </summary>
        [Key]
        public int BusinessId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// InceptionDate
        /// </summary>
        public DateTime InceptionDate { get; set; }

    }
}
