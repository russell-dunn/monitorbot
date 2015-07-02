using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using scbot.core.bot;
using scbot.core.utils;
using scbot.email.services;
using scbot.labelprinting;

namespace scbot.rg
{
    public class CompareTeamEmails : ICommandProcessor, IAcceptEmails
    {
        public static IFeature Create(ICommandParser commandParser, Configuration config)
        {
            var processor = new CompareTeamEmails(
                config.Get("email-channel"), 
                new LabelPrinter(config.Get("printer-api-url"), new WebClient()));

            EmailWatcher.Start(config, new[] {processor});

            return new BasicFeature("emails", "print details of incoming interesting emails into a particular channel", 
                "no commands currently available", 
                new HandlesCommands(commandParser, processor));
        }

        private readonly string m_ChannelToPostEmailsTo;
        private readonly ILabelPrinter m_LabelPrinter;
        private readonly RegexCommandMessageProcessor m_Underlying;
        private readonly ConcurrentQueue<Email> m_Queue = new ConcurrentQueue<Email>();
        private readonly string m_ZendeskLogo = "http://i.imgur.com/kJvbkD9.png";
        private readonly string m_OutlookLogo = "http://i.imgur.com/i0bM6Y9.png";

        public CompareTeamEmails(string channelToPostEmailsTo, ILabelPrinter labelPrinter)
        {
            m_ChannelToPostEmailsTo = channelToPostEmailsTo;
            m_LabelPrinter = labelPrinter;
            m_Underlying = new RegexCommandMessageProcessor(Commands);
        }

        private static Dictionary<Regex, MessageHandler> Commands
        {
            get
            {
                return new Dictionary<Regex, MessageHandler>
                {
                    
                };
            }
        }

        public void Accept(Email emailMessage)
        {
            m_Queue.Enqueue(emailMessage);
        }
    
        public MessageResult ProcessTimerTick()
        {
            var relevantInboxes = new HashSet<string>
            {
                "sqlcomparesupport",
                "sqldatacomparesupport",
                "sqlsdksupport",
                "sqlcomparecoreteam"
            };
            var result = new List<Response>();
            Email email;
            while (m_Queue.TryDequeue(out email))
            {
                Trace.WriteLine("Inspecting email "+email.Subject);
                if (!email.IsFirstEmailInConversation)
                {
                    Trace.WriteLine("Skipping reply to a previously-seen email ...");
                    continue;
                }

                var relevantRecipients = email.RecipientEmails
                    .Where(recipient => relevantInboxes.Any(recipient.Contains))
                    .ToList();
                if (!relevantRecipients.Any())
                {
                    Trace.WriteLine("Skipping non-team-relevant email ...");
                    continue;
                }

                var asEscalation = EmailParser.TryGetZendeskEscalation(email.HtmlBody);
                if (asEscalation != null)
                {
                    result.Add(new Response(string.Format("New support escalation for {0}\n**{1}**\n{2}", 
                        asEscalation.Id, email.Subject, email.FormattedBody),
                        m_ChannelToPostEmailsTo, m_OutlookLogo));
                    m_LabelPrinter.PrintLabel(asEscalation.Id + ": " + email.Subject,
                        new List<string> {m_ZendeskLogo});
                }
                else 
                {
                    result.Add(new Response(string.Format("New email sent to {0}\n**{1}**\n{2}", 
                        relevantRecipients.First(), email.Subject, email.FormattedBody), 
                        m_ChannelToPostEmailsTo, m_OutlookLogo));
                    m_LabelPrinter.PrintLabel(email.Subject, email.FormattedBody,
                        new List<string> {m_OutlookLogo});
                }
            }
            return new MessageResult(result);
        }

        public MessageResult ProcessCommand(Command command)
        {
            return m_Underlying.ProcessCommand(command);
        }
    }
}
