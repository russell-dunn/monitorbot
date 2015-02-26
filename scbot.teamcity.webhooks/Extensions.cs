using System.Text.RegularExpressions;

namespace scbot.teamcity.webhooks
{
    public static class Extensions
    {
        public static bool TryMatch(this Regex regex, string input, out Match match)
        {
            match = regex.Match(input);
            return match.Success;
        }
    }
}
