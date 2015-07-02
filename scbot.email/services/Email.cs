using System;
using System.Collections.Generic;

namespace scbot.email.services
{
    public class Email
    {
        public readonly string HtmlBody;
        public readonly string FormattedBody;
        public readonly string Subject;
        public readonly string ID;
        public readonly DateTime DateTimeReceived;
        public readonly List<string> RecipientEmails;
        public readonly bool IsFirstEmailInConversation;

        public Email(string subject, string htmlBody, string formattedBody, string id, DateTime dateTimeReceived, List<string> recipientEmails, bool isFirstEmailInConversation)
        {
            Subject = subject;
            HtmlBody = htmlBody;
            FormattedBody = formattedBody;
            ID = id;
            DateTimeReceived = dateTimeReceived;
            RecipientEmails = recipientEmails;
            IsFirstEmailInConversation = isFirstEmailInConversation;
        }

        public override string ToString()
        {
            return "<email " + Subject + ">";
        }
    }
}