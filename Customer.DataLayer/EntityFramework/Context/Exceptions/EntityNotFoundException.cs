using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.EntityFramework.Context.Exceptions
{
    /// <summary>
    /// Exception that will get raised when searching for an entity on its identifier
    /// and it is not found.
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="entityName">Name of the entity.</param>
        public EntityNotFoundException(object id, string entityName)
        {
            Id = id;
            EntityName = entityName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="entityName">Name of the entity.</param>
        public EntityNotFoundException(string message, object id, string entityName)
            : base(message)
        {
            Id = id;
            EntityName = entityName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="entityName">Name of the entity.</param>
        protected EntityNotFoundException(SerializationInfo info, StreamingContext context, object id, string entityName)
            : base(info, context)
        {
            Id = id;
            EntityName = entityName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="entityName">Name of the entity.</param>
        public EntityNotFoundException(string message, Exception innerException, object id, string entityName)
            : base(message, innerException)
        {
            Id = id;
            EntityName = entityName;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public object Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>The name of the entity.</value>
        public string EntityName { get; set; }
    }
}