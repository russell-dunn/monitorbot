﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Helpers;
using monitorbot.core.bot;

namespace monitorbot.core.utils
{
    public static class Extensions
    {
        public static bool TryGetCommand(this ICommandParser parser, Message message, string expectedCommand, out string args)
        {
            args = null;
            string command;

            if (!parser.TryGetCommand(message, out command)) return false;
            if (!(command.StartsWith(expectedCommand+" ") || command.EndsWith(expectedCommand))) return false;

            args = command.Substring(expectedCommand.Length).Trim();
            return true;
        }

        public static bool IsDefault<T>(this T x)
        {
            return Equals(x, default(T));
        }

        public static bool IsNotDefault<T>(this T x)
        {
            return !Equals(x, default(T));
        }

        public static async Task<dynamic> DownloadJson(this IWebClient webClient, string url, IEnumerable<string> headers = null)
        {
            var json = await webClient.DownloadString(url, headers);
            return Json.Decode(json);
        }

        public static bool TryMatch(this Regex regex, string input, out Match match)
        {
            match = regex.Match(input);
            return match.Success;
        }

        public static string Group(this Match match, string groupName)
        {
            return match.Groups[groupName].ToString();
        }

        public static bool Contains(this string haystack, string needle, StringComparison stringComparison)
        {
            return haystack.IndexOf(needle, stringComparison) != -1;
        }

        public static TimeSpan Minutes(this int num)
        {
            return TimeSpan.FromMinutes(num);
        }

        public static TimeSpan Minute(this int num)
        {
            return TimeSpan.FromMinutes(num);
        }

        public static TimeSpan Seconds(this int num)
        {
            return TimeSpan.FromSeconds(num);
        }

        public static TimeSpan Second(this int num)
        {
            return TimeSpan.FromSeconds(num);
        }
    }
}
