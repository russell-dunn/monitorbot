using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using monitorbot.core.utils;
using Microsoft.Exchange.WebServices.Data;
using monitorbot.labelprinting;
using WebClient = monitorbot.core.utils.WebClient;

namespace monitorbot.email.services
{
    public class EmailWatcher
    {
        public static void Start(Configuration config, IEnumerable<IAcceptEmails> emailHandlers)
        {
            ServicePointManager.ServerCertificateValidationCallback = EwsAuthentication.CertificateValidationCallBack;

            var creds = config.Get("email-auth").Split(':');
            var userName = creds[0];
            var password = creds[1];
            var ewsUrl = config.GetWithDefault("email-ews-url", null);

            var service = new ExchangeService(ExchangeVersion.Exchange2010_SP1)
            {
                Credentials = new WebCredentials(userName, password),
                TraceEnabled = false,
                TraceFlags = TraceFlags.None
            };

            if (ewsUrl != null)
            {
                Trace.WriteLine("Setting EWS url to " + ewsUrl);
                service.Url = new Uri(ewsUrl);
            }
            else
            {
                Trace.WriteLine("Starting autodiscover for EWS url...");
                service.AutodiscoverUrl(userName, EwsAuthentication.RedirectionUrlValidationCallback);
                Trace.WriteLine(string.Format("Got {0} for EWS url", service.Url));
            }

            SubscribeToNewMails(service, emailHandlers.ToList());

            Trace.WriteLine("Waiting for EWS notifications...");
        }

        private static void SubscribeToNewMails(ExchangeService service, List<IAcceptEmails> emailHandlers)
        {
            var subscription = service.SubscribeToStreamingNotifications(new[] { new FolderId(WellKnownFolderName.Inbox) },
                EventType.NewMail);

            var connection = new StreamingSubscriptionConnection(service, 30);
            connection.AddSubscription(subscription);

            connection.OnNotificationEvent += (StreamingSubscriptionConnection.NotificationEventDelegate) ((sender, args) =>
            {
                foreach (var e in args.Events)
                {
                    var itemEvent = e as ItemEvent;
                    if (itemEvent != null)
                    {
                        Trace.WriteLine(string.Format("Got {0} event with ItemId {1} ...", itemEvent.EventType, itemEvent.ItemId));
                        var email = EmailMessage.Bind(service, itemEvent.ItemId);
                        Trace.WriteLine(string.Format("... is email with subject {0}", email.Subject));

                        var parsedEmail = new Email(email.Subject,
                            email.Body,
                            EmailParser.GetSlackFormattedSummary(email.Body),
                            email.Id.UniqueId,
                            email.DateTimeReceived,
                            email.ToRecipients.Select(x => x.Address.ToLowerInvariant()).ToList(),
                            IsFirstEmailInConversation(email));

                        foreach (var handler in emailHandlers)
                        {
                            handler.Accept(parsedEmail);
                        }
                    }
                    else
                    {
                        Trace.WriteLine("Unknown EWS event type " + e.GetType().Name);
                    }
                }
            });
            connection.OnDisconnect += (sender, args) =>
            {
                Trace.WriteLine("EWS subscription disconnected: " + args.Exception);
                ((StreamingSubscriptionConnection)sender).Open();
                Trace.WriteLine("EWS subscription reconnected");
            };
            connection.OnSubscriptionError += (sender, args) =>
            {
                Trace.WriteLine("EWS subscription ERROR: " + args.Exception);
                while (true)
                {
                    Trace.WriteLine("Trying to resubscribe ...");
                    try
                    {
                        SubscribeToNewMails(service, emailHandlers);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                    }
                }
            };
            connection.Open();
        }

        private static bool IsFirstEmailInConversation(EmailMessage email)
        {
            // The conversation index is defined as a byte array where the first 22 bytes define a 'parent'
            // header and subsequent sets of five bytes define 'child' relationships 
            // eg an index with parent (22) + child1 (5) + child2 (5) indicates a reply to a reply to the 
            // parent email

            // this is all tracked locally - the parent index is generated the first time we see an email,
            // not necessarily for the first email in a chain

            // see https://msdn.microsoft.com/en-us/library/cc765583(office.14).aspx for more info

            // the upshot is that if the index length is 22 then there are no children in the index
            // and this email starts a conversation
            return email.ConversationIndex.Length == 22;
        }
    }
}
