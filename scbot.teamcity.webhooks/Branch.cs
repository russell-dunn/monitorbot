﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot.teamcity.webhooks.tests;

namespace scbot.teamcity.webhooks
{
    struct Branch
    {
        public readonly TeamcityEventTypes TrackedEventTypes;
        public readonly string Name;
        public readonly string Repo;

        public Branch(TeamcityEventTypes trackedEventTypes, string name, string repo = null)
        {
            TrackedEventTypes = trackedEventTypes;
            Name = name;
            Repo = repo;
        }
    }
}
