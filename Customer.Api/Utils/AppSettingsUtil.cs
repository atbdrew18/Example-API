using Customer.Api.DataLayer.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.Utils
{
    /// <summary>
    /// This class assists with pulling values from AppSettings
    /// </summary>
    public class AppSettingsUtil : BaseAppSettingsUtil, IAppSettingsUtil
    {
        #region Internals
        private readonly ILogger<AppSettingsUtil> _logger;
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
        public AppSettingsUtil(ILogger<AppSettingsUtil> logger, IConfiguration configuration)
            : base(logger, configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        /// <summary>
        /// Authentication HMAC App Id
        /// </summary>
        /// <returns></returns>
        public string GetAuthHmacAppId()
        {
            var value = _configuration.GetValue<string>("AppSettings:HmacAppId");
            return value;
        }

        /// <summary>
        /// Authentication HMAC API Key 1
        /// </summary>
        /// <returns></returns>
        public string GetAuthHmacApiKey1()
        {
            var value = _configuration.GetValue<string>("AppSettings:HmacAPIKey1");
            return value;
        }

        /// <summary>
        /// Authentication HMAC API Key 2
        /// </summary>
        /// <returns></returns>
        public string GetAuthHmacApiKey2()
        {
            var value = _configuration.GetValue<string>("AppSettings:HmacAPIKey2");
            return value;
        }

    }
}
