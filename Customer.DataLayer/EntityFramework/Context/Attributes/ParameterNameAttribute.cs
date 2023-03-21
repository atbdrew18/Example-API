using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.EntityFramework.Context.Attributes
{
    /// <summary>
    /// Attribute to tie SQLRequest property to stored procedure parameter.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterNameAttribute : Attribute
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
        /// Creates a ParameterName annotation
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        public ParameterNameAttribute(string name)
        {
            this._name = name;
        }

    }
}
