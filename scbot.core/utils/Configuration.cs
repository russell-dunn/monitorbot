using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace scbot.core.utils
{
    public class Configuration
    {
        private readonly Dictionary<string, string> m_Values;

        public static Configuration Load()
        {
            var dict = new Dictionary<string, string>();
            foreach (var configItem in ConfigurationManager.AppSettings.AllKeys)
            {
                var value = ConfigurationManager.AppSettings[configItem];
                if (value != "FIXME")
                {
                    dict.Add(configItem, value);
                }
            }
            return new Configuration(dict);
        }

        private Configuration(Dictionary<string, string> values)
        {
            m_Values = values;
        }

        public string SlackApiKey { get { return Get("slack-api-key"); } }

        public string RedgateId { get { return Get("redgate-id"); } }

        public string TeamcityApiBase { get { return Get("teamcity-api-base"); } }

        public string TeamcityWebhooksEndpoint { get { return Get("teamcity-webhooks-endpoint"); } }

        public string GithubToken { get { return Get("github-token"); } }

        public string GithubDefaultRepo { get { return Get("github-default-repo"); } }

        public string GithubDefaultUser { get { return Get("github-default-user"); } }

        public string LoggingEndpoint { get { return Get("logging-dashboard-endpoint"); } }

        public string WebcamAuth { get { return Get("webcam-auth"); } }

        public string TeamcityCredentials { get { return Get("teamcity-auth"); } }

        public string LabelPrinterApiUrl { get { return Get("printer-api-url"); } }

        private string Get(string configKey)
        {
            if (m_Values.ContainsKey(configKey))
            {
                return m_Values[configKey];
            }
            throw new Exception(configKey + " needs to be set in config");
        }
    }
}
