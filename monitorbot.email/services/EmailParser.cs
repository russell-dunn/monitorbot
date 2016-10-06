using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace monitorbot.email.services
{
    public static class EmailParser
    {
        public static bool IsZendeskEscalation(string messageText)
        {
            return TryGetZendeskEscalation(messageText) != null;
        }

        public static ZendeskEscalation TryGetZendeskEscalation(string messageText)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(messageText);
            // zendesk escalation emails handily include a bit of microdata we can use
            var actionLinks = doc.DocumentNode.SelectNodes("//div[@itemtype='http://schema.org/ViewAction']/link");
            if (actionLinks == null) return null;

            return actionLinks.Select(x => x.GetAttributeValue("href", null))
                .Where(x => HasDomain(x, "redgatesupport.zendesk.com"))
                .Select(x => new ZendeskEscalation(x))
                .FirstOrDefault();
        }

        public class ZendeskEscalation
        {
            public readonly string Url;
            public readonly string Id;

            public ZendeskEscalation(string url)
            {
                Url = url;
                Id = "ZD#" + url.Split('/').LastOrDefault();
            }
        }

        private static bool HasDomain(string href, string domain)
        {
            Uri uri;
            return Uri.TryCreate(href, UriKind.Absolute, out uri) && uri.Host == domain;
        }

        public static string GetSlackFormattedSummary(string messageText)
        {
            var result = new StringBuilder();
            var doc = new HtmlDocument();
            doc.LoadHtml(messageText);
            var documentNode = doc.DocumentNode;
            GetSlackFormattedSummary(result, documentNode);

            return Regex.Replace(result.ToString().Replace("\r\n", "\n"), "\\n\\s*\\n", "\n").Trim();
        }

        private static void GetSlackFormattedSummary(StringBuilder result, HtmlNode nextNode)
        {
            var text = nextNode.InnerText;
            switch (nextNode.Name)
            {
                case "#comment":
                case "meta":
                case "style":
                    break;
                case "#text":
                    if (ShouldSkipText(text))
                    {
                        break;
                    }
                    result.Append(text.Replace("\r\n", ""));

                    break;
                case "p":
                    if (ShouldSkipText(text))
                    {
                        break;
                    }
                    Recurse(result, nextNode);
                    result.AppendLine();
                    break;
                case "td":
                    Recurse(result, nextNode);
                    result.Append(" ");
                    break;
                case "tr":
                    Recurse(result, nextNode);
                    result.Append("\n");
                    break;
                case "strong":
                    if (ShouldSkipText(text))
                    {
                        break;
                    }
                    result.Append("*");
                    Recurse(result, nextNode);
                    result.Append("*");
                    break;
                case "a":
                    var href = nextNode.GetAttributeValue("href", null);
                    if (href == null)
                    {
                        Recurse(result, nextNode);
                    }
                    else
                    {
                        result.Append("<");
                        result.Append(href);
                        result.Append("|");
                        Recurse(result, nextNode);
                        result.Append(">");
                    }
                    break;
                default:
                    Recurse(result, nextNode);
                    break;
            }
        }

        private static bool ShouldSkipText(string text)
        {
            if (text.StartsWith("##-") && text.EndsWith("-##")) return true;
            if (text.Contains("The Customer account priority")) return true;
            if (text.EndsWith("would like some help with a ticket.")) return true;
            if (text.StartsWith("Please respond with one of these")) return true;
            if (text == "Private note") return true;
            if (text.Contains("You are an agent.")) return true;
            if (text.Contains("This email is a service from Support.")) return true;
            return false;
        }

        private static void Recurse(StringBuilder result, HtmlNode nextNode)
        {
            foreach (var element in nextNode.ChildNodes)
            {
                GetSlackFormattedSummary(result, element);
            }
        }
    }
}
