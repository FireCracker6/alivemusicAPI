using RazorEngine.Templating;
using RazorEngine;
using SendGrid;
using SendGrid.Helpers.Mail;

using CollaborateMusicAPI.Views.EmailTemplates;

namespace CollaborateMusicAPI.Services.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ISendGridClient _client;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _client = new SendGridClient(_configuration["SendGrid:ApiKey"]);
        
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string callbackUrl)
    {
   
        // Assuming your Razor template is in a file named "PasswordResetEmailTemplate.cshtml"
        // and is located in a folder accessible by your application

        var templatePath = "Views/EmailTemplates/PasswordResetEmailTemplate.cshtml";
        var template = File.ReadAllText(templatePath);

        // Create a dynamic object for the model
        var model = new { Name = "Alive user" + toEmail.ToString(), ResetUrl = callbackUrl };

        // Use the dynamic model for rendering
        string emailBody = Engine.Razor.RunCompile(template, "templateKey", null, model);


        // SendGrid email setup
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("leah@alivemusic.se", "Alive Music"),
            Subject = "Password Reset Request",
            HtmlContent = emailBody
        };
        msg.AddTo(new EmailAddress(toEmail));

        return _client.SendEmailAsync(msg);
    }

}
