﻿using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace monitorbot.jira.services
{
    public class JiraApi : IJiraApi
    {
        private const string c_ApiBase = "https://jira.red-gate.com/rest/api/2/issue/";

        private async Task<string> ApiCall(string url)
        {
            using (var webClient = new WebClient())
            {
                return await webClient.DownloadStringTaskAsync(url);
            }
        }

        private async Task<JiraBug> FromApi(string id)
        {
            var json = await ApiCall(c_ApiBase + id);
            var obj = Json.Decode(json);
            return new JiraBug(id, obj.fields.issuetype.name, obj.fields.summary, obj.fields.description, obj.fields.status.name, obj.fields.comment.total);
        }

        public async Task<JiraBug> FromId(string id)
        {
            try
            {
                return await FromApi(id);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }
    }
}