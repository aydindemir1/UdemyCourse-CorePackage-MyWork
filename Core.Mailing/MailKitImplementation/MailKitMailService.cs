using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Mailing.MailKitImplementation
{
    public class MailKitMailService : IMailService
    {
        private readonly IConfiguration _configuration;
        private readonly MailSettings _mailSettings;

        public MailKitMailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _mailSettings = configuration.GetSection("MailSettings").Get<MailSettings>();
        }

        public async Task SendMailAsync(Mail mail)
        {
            if (mail.ToList?.Any() != true)
                return;

            MimeMessage email = new MimeMessage();

            email.From.Add(new MailboxAddress(_mailSettings.SenderFullName, _mailSettings.SenderEmail));

            email.To.AddRange(mail.ToList);

            email.Subject = mail.Subject;

            BodyBuilder bodyBuilder = new BodyBuilder
            {
                TextBody = mail.TextBody,
                HtmlBody = mail.HtmlBody
            };

            if (mail.Attachments != null)
            {
                foreach (MimeEntity attachment in mail.Attachments)
                    bodyBuilder.Attachments.Add(attachment);
            }

            email.Body = bodyBuilder.ToMessageBody();

            using SmtpClient smtpClient = new SmtpClient();

            SecureSocketOptions socketOptions = (SecureSocketOptions)_mailSettings.SecureOption;

            await smtpClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port, socketOptions);

            if (_mailSettings.Authentication)
                await smtpClient.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);

            await smtpClient.SendAsync(email);

            smtpClient.Disconnect(true);

            email.Dispose();

            smtpClient.Dispose();

        }
    }
}
