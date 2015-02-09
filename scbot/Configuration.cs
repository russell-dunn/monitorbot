using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot
{
    public static class Configuration
    {
        public static string SlackApiKey
        {
            get { return GetConfigValue("slack-api-key"); }
        }

        private static string GetConfigValue(string configKey)
        {
            var value = ConfigurationManager.AppSettings[configKey];
            if (String.IsNullOrWhiteSpace(value) || value == "FIXME")
            {
                var appConfig = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                throw new Exception(configKey + " needs to be set in app.config at " + appConfig);
            }
            return value;
        }
    }
}
