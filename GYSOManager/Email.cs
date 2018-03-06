using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace GYSOManager
{
    /// <summary>
    /// Uses the GYSO gmail account.
    /// </summary>
    public class Email
    {
        public const string Server = "smtp.gmail.com";
        public const int Port = 587;

        public static void SendMessage(string recipient, string body)
        {
            var client = new SmtpClient(Server, Port);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["email"], ConfigurationManager.AppSettings["email-password"]);
            client.EnableSsl = true;

            var message = new MailMessage();
            message.From = new MailAddress(ConfigurationManager.AppSettings["email"]);
            message.To.Add(recipient);
            message.Body = body;
            message.Subject = "GYSO Registration";

            client.Send(message);
        }
    }
}