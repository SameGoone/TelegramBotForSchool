using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace TelegramBotForSchool
{
    public class EmailController
    {
        public static EmailController Singleton { get; private set; }

        private string fileName = "email-config.txt";

        private string fromEmail;
        private string fromPassword;

        public EmailController()
        {
            if (Singleton == null)
                Singleton = this;

            ReadConfigsFromFile();
        }

        private void ReadConfigsFromFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    fromEmail = sr.ReadLine();
                    fromPassword = sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendNotification(string ToEmail, string result)
        {
            MailAddress from = new MailAddress(fromEmail);
            MailAddress to = new MailAddress(ToEmail);

            MailMessage m = new MailMessage(from, to);
            m.Subject = "Новая заявка Школа 2086.";
            m.Body = "Получена новая заявка от сотрудника ГБОУ Школа 2086.\r\n" 
                + result 
                + $"\r\nЧтобы просмотреть таблицу, перейдите по ссылке: {SpreadsheetsController.Singleton.SheetURL}";

            
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                Timeout = 20000
            };
            smtp.Send(m);
            Console.WriteLine($"Отправлено уведомление на адрес {m.To.First().Address}");
        }
    }
}
