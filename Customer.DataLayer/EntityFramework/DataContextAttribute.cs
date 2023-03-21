using System;

namespace Customer.Api.DataLayer.EntityFramework
{
    /// <summary>
    /// Attribute to set DataContext connection name on entities.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class)]
    public class DataContextAttribute : Attribute
    {

        /// <summary>
        /// DataContext Name
        /// </summary>
        private string _dataContextName;

        /// <summary>
        /// Gets the name of the DataContenxt / ConnectionString to Use
        /// </summary>
        public string DataContextName
        {
            get
            {
                return _dataContextName;
            }
        }

        /// <summary>
        /// Creates a DataContext annotation
        /// </summary>
        /// <param name="name">The name of the connection string to use</param>
        public DataContextAttribute(string name)
        {
            this._dataContextName = name;
        }
    }
}
