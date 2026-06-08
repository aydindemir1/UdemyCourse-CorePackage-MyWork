using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Mailing
{
    public class Mail
    {
        public string Subject { get; set; }
        public string TextBody { get; set; }
        public string HtmlBody { get; set; }
        public AttachmentCollection? Attachments { get; set; }
        public List<MailboxAddress> ToList { get; set; }

        public Mail()
        {

        }

        public Mail(string subject, string textBody, string htmlBody, AttachmentCollection? attachments, List<MailboxAddress> toList)
        {
            Subject = subject;
            TextBody = textBody;
            HtmlBody = htmlBody;
            Attachments = attachments;
            ToList = toList;
        }
    }
}
