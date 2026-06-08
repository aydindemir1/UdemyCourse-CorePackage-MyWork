using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Mailing
{
    public class MailSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderFullName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Authentication { get; set; }
        public int SecureOption { get; set; }

        public MailSettings()
        {

        }

        public MailSettings(string server, int port, string senderFullName, string senderEmail, string username, string password, bool authentication)
        {
            Server = server;
            Port = port;
            SenderFullName = senderFullName;
            SenderEmail = senderEmail;
            Username = username;
            Password = password;
            Authentication = authentication;
        }
    }
}
