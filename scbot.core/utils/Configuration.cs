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

        public string Get(string configKey)
        {
            if (m_Values.ContainsKey(configKey))
            {
                return m_Values[configKey];
            }
            throw new Exception(configKey + " needs to be set in config");
        }
    }
}
