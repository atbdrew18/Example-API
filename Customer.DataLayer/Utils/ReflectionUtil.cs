using Customer.Api.DataLayer.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.Utils
{
    public class ReflectionUtil
    {
        public static T CopyCommonEntityProperties<T>(object inputObject, T outputObject, List<string> propertiesToSkip, bool ignoreVirtual = true) 
            where T : BaseEntity
        {
            if (propertiesToSkip == null) propertiesToSkip = new List<string>();

            var propertiesCopy = inputObject.GetType().GetProperties();

            if (ignoreVirtual)
            {
                foreach (var property in propertiesCopy)
                {
                    if (property.GetGetMethod().IsVirtual)
                    {
                        propertiesToSkip.Add(property.Name);
                    }
                }
            }

            foreach (var property in propertiesCopy)
            {
                if (!propertiesToSkip.Contains(property.Name))
                {
                    var value = inputObject.GetType().GetProperty(property.Name).GetValue(inputObject, null);

                    var propertyoutput = outputObject.GetType().GetProperty(property.Name);
                    if (propertyoutput != null)
                    {
                        outputObject.GetType().GetProperty(property.Name).SetValue(outputObject, value, null);
                    }
                }
            }

            return outputObject;
        }
    }
}
