using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.EntityFramework.Context.Attributes
{
    /// <summary>
    /// Attribute to tie class extending SQLRequest to stored procedure by name.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class)]
    public class ProcedureNameAttribute : Attribute
    {

        /// <summary>
        /// Parameter Name
        /// </summary>
        private string _name;

        /// <summary>
        /// Parameter Name
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Creates a Procedure Name annotation
        /// </summary>
        /// <param name="name">The name of the stored procedure</param>
        public ProcedureNameAttribute(string name)
        {
            this._name = name;
        }

    }
}
