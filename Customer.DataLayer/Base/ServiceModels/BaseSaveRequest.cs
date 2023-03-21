using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.Base.ServiceModels
{
    public class BaseSaveRequest<T> : BaseRequest
    {
        #region Functions
        /// <summary>
        /// Get the Key from the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetId<T>()
        {
            var props = this.Entity.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(KeyAttribute))).ToList();

            if (props != null && props.Count > 0)
            {
                return (T?)props[0].GetValue(this.Entity);
            }

            return default;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Entity
        /// </summary>
        public T? Entity { get; set; }
        #endregion
    }
}
