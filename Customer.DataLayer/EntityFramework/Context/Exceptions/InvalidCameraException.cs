using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.EntityFramework.Context.Exceptions
{
    [Serializable]
    public class InvalidCameraException : Exception
    {
        public InvalidCameraException()
        { }

        public InvalidCameraException(string message)
            : base(message)
        { }

        public InvalidCameraException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
