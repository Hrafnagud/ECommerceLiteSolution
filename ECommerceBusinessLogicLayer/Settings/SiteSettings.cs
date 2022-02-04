﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ECommerceLiteEntity.ViewModels;

namespace ECommerceBusinessLogicLayer.Settings
{
    public static class SiteSettings
    {
        public static string SiteMail { get; set; } = "yazilim103@gmail.com";
        public static string SiteMailPasssword { get; set; } = "betul103103";

        public static string SiteMailSmtpHost = "smtp.gmail.com";

        public static int SiteMailSmtpPort = 587;

        public static bool SiteMailEnableSsl = true;

        public async static Task SendMail(MailModel model)
        {
            try
            {
                using (var smtp = new SmtpClient())
                {
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(model.To));
                    message.From = new MailAddress(SiteMail);
                    message.Subject = model.Subject;
                    message.IsBodyHtml = true;
                    message.Body = model.Message;
                    message.BodyEncoding = Encoding.UTF8;

                    if (!string.IsNullOrEmpty(model.Cc))
                    {
                        message.CC.Add(new MailAddress(model.Cc));
                    }

                    if (!string.IsNullOrEmpty(model.Bcc))
                    {
                        message.Bcc.Add(new MailAddress(model.Bcc));
                    }

                    var credential = new NetworkCredential()
                    {
                        UserName = SiteMail,
                        Password = SiteMailPasssword
                    };

                    smtp.Credentials = credential;
                    smtp.Host = SiteMailSmtpHost;
                    smtp.Port = SiteMailSmtpPort;
                    smtp.EnableSsl = SiteMailEnableSsl;
                    await smtp.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                //TODO: ex log
            }
        }

        public static void SendMail()
        {

        }

        public static string UrlFormatConverter(string name)
        {
            string result = name.ToLower()
            .Replace("'", "")
            .Replace(" ", "-")
            .Replace("<", "")
            .Replace(">", "")
            .Replace("&", "")
            .Replace("[", "")
            .Replace("!", "")
            .Replace("]", "")
            .Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ç", "c")
            .Replace("ğ", "g")
            .Replace("İ", "I")
            .Replace("Ö", "O")
            .Replace("Ü", "U")
            .Replace("Ş", "S")
            .Replace("Ç", "C")
            .Replace("Ğ", "G")
            .Replace("|", "")
            .Replace(".", "-")
            .Replace("?", "-")
            .Replace(";", "-");

            return result;
        }
    }
}
