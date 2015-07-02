using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Helpers;

namespace scbot.core.utils
{
    public class Configuration
    {
        private readonly Dictionary<string, string> m_Values;

        public static Configuration Load()
        {
            var pathToConfig = Environment.GetEnvironmentVariable("SCBOT_CONFIG_FILE") ?? "config.json";
            Trace.WriteLine("Loading config from " + pathToConfig);
            var dict = Json.Decode<Dictionary<string, string>>(File.ReadAllText(pathToConfig));
            return new Configuration(dict);
        }

        private Configuration(Dictionary<string, string> values)
        {
            m_Values = values;
        }

        public string Get(string configKey)
        {
            if (m_Values.ContainsKey(configKey))
            {
                return m_Values[configKey];
            }
            throw new Exception(configKey + " needs to be set in config");
        }

        public string GetWithDefault(string configKey, string defaultValue)
        {
            if (m_Values.ContainsKey(configKey))
            {
                return m_Values[configKey];
            }
            return defaultValue;
        }
    }
}
