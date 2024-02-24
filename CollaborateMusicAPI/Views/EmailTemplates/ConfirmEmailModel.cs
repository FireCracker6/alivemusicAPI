namespace CollaborateMusicAPI.Views.EmailTemplates;

public class ConfirmEmailModel
{
    public string Email { get; set; }
    public string ConfirmEmailUrl { get; set; }
    public string ResetUrl { get; set; } = null!;
}
