using MailKit.Net.Smtp;
using MimeKit;

namespace Acount.Helpers
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com"; 
        private readonly int _smtpPort = 587;
        private readonly string _emailFrom = "youssefbadrtast@gmail.com";
        private readonly string _emailPassword = "bxof xwti hasx sgsx";

        public async Task SendEmailAsync(string emailTo, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Account App ", _emailFrom));
            emailMessage.To.Add(new MailboxAddress("", emailTo));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart("plain") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, false);
                await client.AuthenticateAsync(_emailFrom, _emailPassword);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
