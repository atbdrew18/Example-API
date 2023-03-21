using Customer.Api.DataLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.Utils
{
    public interface IAppSettingsUtil : IBaseAppSettingsUtil
    {
        public string GetAuthHmacAppId();
        public string GetAuthHmacApiKey1();
        public string GetAuthHmacApiKey2();
    }
}
