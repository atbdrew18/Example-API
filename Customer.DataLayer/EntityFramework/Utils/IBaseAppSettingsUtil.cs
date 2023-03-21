using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.Utils
{
    public interface IBaseAppSettingsUtil
    {
        public String GetConnectionStringDefaultDb();
    }
}
