using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace scbot.services
{
    class ReconnectingZendeskApi : IZendeskTicketApi
    {
        private readonly Func<Task<IZendeskTicketApi>> m_ZdApiFactory;
        private IZendeskTicketApi m_ZdApi;

        private ReconnectingZendeskApi(IZendeskTicketApi zdApi, Func<Task<IZendeskTicketApi>> zdApiFactory)
        {
            m_ZdApiFactory = zdApiFactory;
            m_ZdApi = zdApi;
        }

        public static async Task<ReconnectingZendeskApi> CreateAsync(Func<Task<IZendeskTicketApi>> zdApiFactory)
        {
            var api = await zdApiFactory();
            return new ReconnectingZendeskApi(api, zdApiFactory);
        }

        public async Task<ZendeskTicket> FromId(string id)
        {
            try
            {
                return await m_ZdApi.FromId(id);
            }
            catch (WebException we)
            {
                var httpResponse = we.Response as HttpWebResponse;
                if (httpResponse == null || httpResponse.StatusCode != HttpStatusCode.Unauthorized)
                {
                    throw;
                }
            }

            // You can trigger this code by going to the Zendesk profile page, 
            // selecting the Devices and apps tab and deleting other sessions
            Console.WriteLine("Caught 401 from zendesk .. attempting to reauth");
            // reconnect and retry once
            m_ZdApi = await m_ZdApiFactory();
            return await m_ZdApi.FromId(id);
        }
    }
}
