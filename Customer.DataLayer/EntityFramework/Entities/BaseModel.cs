using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.EntityFramework.Entities
{
    public class BaseModel : ICloneable
    {
        /// <summary>
        /// Perform a Shallow Clone of the current object
        /// </summary>
        /// <returns>a new Instance of the current object</returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
