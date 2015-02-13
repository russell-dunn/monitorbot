using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web.Helpers;

namespace scbot.services
{
    public class JsonFileKeyValueStore : IKeyValueStore
    {
        private readonly FileInfo m_File;

        public JsonFileKeyValueStore(FileInfo file)
        {
            m_File = file;
        }

        public void Set(string key, string value)
        {
            var db = ReadDb();
            db[key] = value;
            File.WriteAllText(m_File.FullName, Json.Encode(db));
        }

        private dynamic ReadDb()
        {
            if (!m_File.Exists) File.WriteAllText(m_File.FullName, "");
            return Json.Decode(File.ReadAllText(m_File.FullName)) ?? new DynamicJsonObject(new Dictionary<string, object>());
        }

        public string Get(string key)
        {
            var db = ReadDb();
            //there must be a better way of doing this
            if (((DynamicJsonObject)db).GetDynamicMemberNames().Contains(key))
            {
                return db[key];
            }
            return null;
        }
    }
}