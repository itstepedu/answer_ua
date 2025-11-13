using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AnswerUA.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;

namespace AnswerUA.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Логування в консоль
            Console.WriteLine($"FROM EMAIL = '{_emailSettings.FromEmail}'");
            Console.WriteLine($"SMTP USER = '{_emailSettings.SmtpUser}'");
            Console.WriteLine($"TO EMAIL = '{email}'");

            // Логування в файл
            File.AppendAllText("email-log.txt", 
                $"SendEmailAsync - FROM={_emailSettings.FromEmail}, TO={email}, SUBJECT={subject}\n");

            using var smtp = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass),
                EnableSsl = true
            };

            MailMessage messageBody;

            try
            {
                messageBody = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                messageBody.To.Add(email);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Некоректний email адрес: '{email}' або FromEmail '{_emailSettings.FromEmail}'");
                File.AppendAllText("email-log.txt", 
                    $"ERROR: Некоректний email - TO={email}, FROM={_emailSettings.FromEmail}\n");
                throw;
            }

            await smtp.SendMailAsync(messageBody);
        }

        public async Task SendEmailWithReply(string email, string subject, string message, string replyToEmail, string replyToName)
        {
            // Логування в консоль
            Console.WriteLine($"FROM EMAIL = '{_emailSettings.FromEmail}'");
            Console.WriteLine($"SMTP USER = '{_emailSettings.SmtpUser}'");
            Console.WriteLine($"TO EMAIL = '{email}'");
            Console.WriteLine($"REPLY-TO EMAIL = '{replyToEmail}'");

            // Логування в файл
            File.AppendAllText("email-log.txt", 
                $"SendEmailWithReply - FROM={_emailSettings.FromEmail}, TO={email}, REPLYTO={replyToEmail}, SUBJECT={subject}\n");

            using var smtp = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass),
                EnableSsl = true
            };

            MailMessage messageBody;

            try
            {
                messageBody = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                messageBody.To.Add(email);

                if (!string.IsNullOrEmpty(replyToEmail))
                {
                    messageBody.ReplyToList.Add(new MailAddress(replyToEmail, replyToName));
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Некоректний email: TO='{email}', FROM='{_emailSettings.FromEmail}', REPLYTO='{replyToEmail}'");
                File.AppendAllText("email-log.txt", 
                    $"ERROR: Некоректний email - TO={email}, FROM={_emailSettings.FromEmail}, REPLYTO={replyToEmail}\n");
                throw;
            }

            await smtp.SendMailAsync(messageBody);
        }
    }
}
