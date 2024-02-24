namespace CollaborateMusicAPI.Services.Email
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetToken);
        Task SendWelcomeEmailAsync(string toEmail, string subject, string userEmail);
    }
}
