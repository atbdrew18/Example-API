using Customer.Api.DataLayer.Helpers.Encryption;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Customer.Api.DataLayer.EntityFramework.Entities
{
    [DataContextAttribute("DefaultConnection")]
    public class Customer : BaseEntity
    {
        /// <summary>
        /// CustomerId
        /// </summary>
        [Key]
        public int CustomerId { get; set; }

        /// <summary>
        /// FirstName
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// LastName
        /// </summary>
        public string LastName { get; set; }
    }
}
