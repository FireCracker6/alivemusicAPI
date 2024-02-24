
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using RazorEngine.Templating;
using RazorEngine;
using SendGrid;
using CollaborateMusicAPI.Views.EmailTemplates;

namespace CollaborateMusicAPI.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ISendGridClient _client;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _client = new SendGridClient(_configuration["SendGrid:ApiKey"]);
            _logger = logger;
        }

        public Task SendPasswordResetEmailAsync(string toEmail, string callbackUrl)
        {
            var host = _configuration["SmtpSettings:Host"];
            var port = int.Parse(_configuration["SmtpSettings:Port"]);
            var username = _configuration["SmtpSettings:Username"];
            var password = _configuration["SmtpSettings:Password"];
            var enableSsl = bool.Parse(_configuration["SmtpSettings:UseSsl"]);


            var templatePath = "Views/EmailTemplates/PasswordResetEmailTemplate.cshtml";
            var templateFullPath = Path.Combine(Directory.GetCurrentDirectory(), templatePath);

            var template = File.ReadAllText(templateFullPath);
            var model = new { Name = "User's Name", ResetUrl = callbackUrl };
            string emailBody = Engine.Razor.RunCompile(template, "templateKey", null, model);

            // SMTP email setup
            var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("info@alivemusic.se", "Alive Music"),
                Subject = "Password Reset Request",
                Body = emailBody,
                IsBodyHtml = true,
                Priority = MailPriority.High
            };
            mailMessage.To.Add(toEmail);

            return smtpClient.SendMailAsync(mailMessage);
        }
        public async Task SendWelcomeEmailAsync(string toEmail, string subject, string userEmail)
        {
            try
            {
                var host = _configuration["SmtpSettings:Host"];
                var port = int.Parse(_configuration["SmtpSettings:Port"]);
                var username = _configuration["SmtpSettings:Username"];
                var password = _configuration["SmtpSettings:Password"];
                var enableSsl = bool.Parse(_configuration["SmtpSettings:UseSsl"]);

                var templatePath = "Views/EmailTemplates/WelcomeEmailTemplate.cshtml";
                var templateFullPath = Path.Combine(Directory.GetCurrentDirectory(), templatePath);
                var template = File.ReadAllText(templateFullPath);

                var model = new ConfirmEmailModel
                {
                    Email = userEmail,
                    ConfirmEmailUrl = "http://localhost:3000" // You need to provide the appropriate URL
                };

                string emailBody = Engine.Razor.RunCompile(template, "templateKey", typeof(ConfirmEmailModel), model);





                var smtpClient = new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("info@alivemusic.se", "Alive Music"),
                    Subject = subject,
                    Body = emailBody,
                    IsBodyHtml = true,
                    Priority = MailPriority.High
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or throw
                // For example: 
                _logger.LogError(ex, "Error occurred while sending welcome email to: {Email}. Exception: {ExceptionMessage}", toEmail, ex.ToString());

                throw; // Or handle it as per your application's error handling policy
            }
        }

        //public async Task SendWelcomeEmailAsync(string toEmail, string subject, string userEmail)
        //{
        //    try
        //    {
        //        // SMTP Client setup remains the same
        //        var host = _configuration["SmtpSettings:Host"];
        //        var port = int.Parse(_configuration["SmtpSettings:Port"]);
        //        var username = _configuration["SmtpSettings:Username"];
        //        var password = _configuration["SmtpSettings:Password"];
        //        var enableSsl = bool.Parse(_configuration["SmtpSettings:UseSsl"]);

        //        var smtpClient = new SmtpClient(host)
        //        {
        //            Port = port,
        //            Credentials = new NetworkCredential(username, password),
        //            EnableSsl = enableSsl,
        //        };

        //        // Set a simple plain text message
        //        string emailBody = "This is a test email. If you're reading this, the SMTP settings are correct.";

        //        var mailMessage = new MailMessage
        //        {
        //            From = new MailAddress("info@alivemusic.se", "Alive Music"),
        //            Subject = subject,
        //            Body = emailBody,
        //            IsBodyHtml = false, // Set to false for plain text email
        //            Priority = MailPriority.High
        //        };
        //        mailMessage.To.Add(toEmail);

        //        await smtpClient.SendMailAsync(mailMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while sending test email to: {Email}", toEmail);
        //        throw;
        //    }
        //}


    }

}
