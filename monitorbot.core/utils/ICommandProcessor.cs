using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorbot.core.bot;

namespace monitorbot.core.utils
{
    public interface ICommandProcessor
    {
        MessageResult ProcessTimerTick();
        MessageResult ProcessCommand(Command command);
    }
}
