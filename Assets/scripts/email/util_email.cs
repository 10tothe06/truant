using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


public class util_email
{
    public static void SendEmail(List<string> recipients, string subject, string bodyText) {
        MailMessage messageToSend = new MailMessage();
        SmtpClient StmpServer = new SmtpClient("smtp.gmail.com");
        StmpServer.Timeout = 10000;
        StmpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
        StmpServer.UseDefaultCredentials = false;
        StmpServer.Port = 587;

        StmpServer.EnableSsl = true;

        // we're sending an email to me, from me, containing the bug information
        // this will automatically get routed into a label on the GMail side
        messageToSend.From = new MailAddress("maximilianmcdiarmid@gmail.com");
        
        for (int i = 0; i < recipients.Count; i++)
        {
            messageToSend.To.Add(new MailAddress(recipients[i]));
        }

        messageToSend.Subject = subject;
        messageToSend.Body = bodyText;

        StmpServer.Credentials = new NetworkCredential("maximilianmcdiarmid@gmail.com", "tahg sdah jsaf uhln") as ICredentialsByHost;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        };

        messageToSend.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        
        // TODO: error protection
        StmpServer.Send(messageToSend);
    }

    public static void SendEmailToSelf(string subject, string bodyText) {
        MailMessage messageToSend = new MailMessage();
        SmtpClient StmpServer = new SmtpClient("smtp.gmail.com");
        StmpServer.Timeout = 10000;
        StmpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
        StmpServer.UseDefaultCredentials = false;
        StmpServer.Port = 587;

        StmpServer.EnableSsl = true;

        // we're sending an email to me, from me, containing the bug information
        // this will automatically get routed into a label on the GMail side
        messageToSend.From = new MailAddress("maximilianmcdiarmid@gmail.com");
        messageToSend.To.Add(new MailAddress("maximilianmcdiarmid@gmail.com"));

        messageToSend.Subject = subject;
        messageToSend.Body = bodyText;

        StmpServer.Credentials = new NetworkCredential("maximilianmcdiarmid@gmail.com", "INSERT KEY HERE") as ICredentialsByHost;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        };

        messageToSend.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        StmpServer.Send(messageToSend);
    }
}
