using System.Collections.Generic;

namespace scbot.labelprinting
{
    public interface ILabelPrinter
    {
        string PrintLabel(string title, List<string> images);
        string PrintLabel(string title, string text, List<string> images);
    }
}