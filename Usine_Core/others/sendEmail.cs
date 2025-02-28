using System.Net.Mail;
using System.Net;
namespace Usine_Core.others
{
    public class sendEmail
    {
        public string EmailSend(string Subject, string receiver, string message, string fileUrl = null, string cc = null, string cc1 = null)
        {
            var senderEmail = new MailAddress("noreply@cortracker360.com");
            var receiverEmail = new MailAddress(receiver);
            var password = "Noreply@#02024";
            var sub = Subject;
            var body = message;
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mail = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = Subject,
                Body = body

            })
            {
                if (fileUrl != "" && fileUrl != null)
                {
                    mail.Attachments.Add(new Attachment(System.Environment.CurrentDirectory + fileUrl));
                }
                if (cc != "" && cc != null)
                {
                    mail.CC.Add(cc);
                }
                if (cc1 != "" && cc1 != null)
                {
                    mail.CC.Add(cc1);
                }
                smtp.Send(mail);
            }
            return null;

        }
    }
}
