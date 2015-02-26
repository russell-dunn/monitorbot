using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot.services.teamcity;

namespace scbot.teamcity.webhooks
{
    public interface IAcceptTeamcityEvents
    {
        void Accept(TeamcityEvent teamcityEvent);
    }
}
