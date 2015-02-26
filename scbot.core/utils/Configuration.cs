using System;
using System.Configuration;
using System.IO;

namespace scbot.core.utils
{
    public static class Configuration
    {
        public static string SlackApiKey
        {
            get { return GetConfigValue("slack-api-key"); }
        }

        public static string RedgateId
        {
            get { return GetConfigValue("redgate-id"); }
        }

        public static string TeamcityApiBase { get { return GetConfigValue("teamcity-api-base"); } }

        private static string GetConfigValue(string configKey)
        {
            var value = ConfigurationManager.AppSettings[configKey];
            if (String.IsNullOrWhiteSpace(value) || value == "FIXME")
            {
                var appConfig = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                throw new Exception(configKey + " needs to be set in local.config at " + Path.GetDirectoryName(appConfig));
            }
            return value;
        }
    }
}
