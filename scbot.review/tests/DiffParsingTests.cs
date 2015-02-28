using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net;
using scbot.review.diffparser;

namespace scbot.review.tests
{
    class DiffParsingTests
    {
        private void Log(string str) { Console.WriteLine(str); }

        [Test]
        public void CanParseDiff()
        {
            var client = new WebClient();
            client.Encoding = new UTF8Encoding(false);
            var diff = client.DownloadString("https://github.com/nyctef/sweetiebot/commit/6c27dccb66cb93654bbaf27573562ff715f42866.diff");
            List<DiffLine> result = DiffParser.ParseDiff(diff);

            foreach (var l in result) { Log(l.ToString()); }
        }

        
    }
}
