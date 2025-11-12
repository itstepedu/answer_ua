using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AnswerUA.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;

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
            Console.WriteLine($"SMTP user: {_emailSettings.SmtpUser}");
            Console.WriteLine($"SMTP pass: {_emailSettings.SmtpPass?.Substring(0, 3)}***");


            using var smtp = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass),
                EnableSsl = true
            };

            var messageBody = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            Console.WriteLine($"Sending email to {email} with subject {subject}");

            messageBody.To.Add(email);

            await smtp.SendMailAsync(messageBody);
        }

        public async Task SendEmailWithReply(string email, string subject, string message, string replyToEmail, string replyToName)
        {
            Console.WriteLine($"SMTP user: {_emailSettings.SmtpUser}");
            Console.WriteLine($"SMTP pass: {_emailSettings.SmtpPass?.Substring(0, 3)}***");


            using var smtp = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass),
                EnableSsl = true
            };

            var messageBody = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            Console.WriteLine($"Sending email to {email} with subject {subject}");

            messageBody.To.Add(email);

            if (!string.IsNullOrEmpty(replyToEmail))
            {
                messageBody.ReplyToList.Add(new MailAddress(replyToEmail, replyToName));
            }

            await smtp.SendMailAsync(messageBody);
        }
    }
}