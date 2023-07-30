using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

namespace E_Shop.Models
{
    public class MailServices
    {
        public string From = "botickyauth@seznam.cz";

        public void MailSrv(string TargetMail, string Title, string Message)
        {
   

            MailMessage msg = new MailMessage(From, TargetMail, Title, Message);
            msg.IsBodyHtml = true;
            var client = new SmtpClient("smtp.seznam.cz", 25)
            {
                Credentials = new NetworkCredential(From, "mamradboty123"),
                EnableSsl = false
            };

            client.Send(msg);


        }
        public void Order(string TargetMail, string Title, string Message, CustomerData Data)
        {
            DateTime now = DateTime.Now;
            StreamReader R = new StreamReader(@"X:\\email.txt");          
            string Body = R.ReadToEnd();
            Body.Replace("#DATUM",now.AddDays(7).ToString());
            Body = Body.Replace("#CENA", Data.OrderPrice.ToString());
            Body = Body.Replace("#1", Data.Address.ToString());
            Body = Body.Replace("#A2", Data.Address2.ToString());
            Body = Body.Replace("#STAT",Data.Country.ToString());
            Body = Body.Replace("#ID", 641136.ToString());
            MailMessage msg = new MailMessage(From, TargetMail, Title, Body);
            msg.IsBodyHtml = true;
            msg.Body = Body;
            var client = new SmtpClient("smtp.seznam.cz", 25)
            {
                Credentials = new NetworkCredential(From, "mamradboty123"),
                EnableSsl = false
            };

            client.Send(msg);

        }
        public void Cancel(string TargetMail, string Title, string Message, Order Order)
        {
            DateTime now = DateTime.Now;
            StreamReader R = new StreamReader(@"X:\\cancel.txt");
            string Body = R.ReadToEnd();
            Body = Body.Replace("#REASON", Message);
            Body = Body.Replace("#ID", Order.OrderID.ToString());
            MailMessage msg = new MailMessage(From, TargetMail, Title, Body);
            msg.IsBodyHtml = true;
            msg.Body = Body;
            var client = new SmtpClient("smtp.seznam.cz", 25)
            {
                Credentials = new NetworkCredential(From, "mamradboty123"),
                EnableSsl = false
            };

            client.Send(msg);

        }
    }
}
