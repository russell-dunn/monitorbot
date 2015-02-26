using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.teamcity.webhooks
{
    struct Branch
    {
        public readonly string Name;
        public readonly string Repo;

        public Branch(string name, string repo = null)
        {
            Name = name;
            Repo = repo;
        }
    }
}
