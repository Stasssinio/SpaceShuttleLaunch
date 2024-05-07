using System.Net.Mail;

namespace SpaceShuttleLaunch
{
    internal class EmailService(string sender, string password)
    {
        readonly string _sender = sender;
        readonly string _password = password;

        public void SendMail(string recipient, string subject, string message, string reportPath)
        {
            // Using the outlook mail service as recommended
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(_sender, _password);
            client.EnableSsl = true;
            client.Credentials = credentials;

            try
            {
                var mail = new MailMessage(_sender.Trim(), recipient.Trim());
                mail.Subject = subject;
                mail.Body = message;
                mail.Attachments.Add(new Attachment(reportPath));
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
