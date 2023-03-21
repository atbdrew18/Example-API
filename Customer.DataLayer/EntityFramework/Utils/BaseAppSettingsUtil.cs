using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.DataLayer.Utils
{
    /// <summary>
    /// This class assists with pulling values from AppSettings
    /// </summary>
    public class BaseAppSettingsUtil : IBaseAppSettingsUtil
    {
        #region Internals
        private readonly ILogger<BaseAppSettingsUtil> _logger;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="recordingService"></param>
        /// <param name="hostedService"></param>
        public BaseAppSettingsUtil(ILogger<BaseAppSettingsUtil> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        /// <summary>
        /// Get the Default DB Connection String
        /// </summary>
        /// <returns></returns>
        public String GetConnectionStringDefaultDb()
        {
            var value = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            return value;
        }
    }
}
