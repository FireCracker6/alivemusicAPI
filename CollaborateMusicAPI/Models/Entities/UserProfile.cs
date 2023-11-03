using CollaborateMusicAPI.Contexts;

namespace CollaborateMusicAPI.Models.Entities
{
    public class UserProfile
    {
        public int UserProfileID { get; set; }
        public string FullName { get; set; } = null!;
        public string? Bio { get; set; } 
        public string? ProfilePicturePath { get; set; }
        public string? Location { get; set; }
        public string? WebsiteURL { get; set; }

        public Guid UserID { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<Artist> Artists { get; set; } = null!;

    }
}
