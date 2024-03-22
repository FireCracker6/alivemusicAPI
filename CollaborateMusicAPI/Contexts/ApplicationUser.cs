using ALIVEMusicAPI.Models.Entities;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace CollaborateMusicAPI.Contexts
{
    public class ApplicationUser : IdentityUser<Guid>  // Notice the <Guid> generic parameter
    {
        // No need to redefine the Id property. It's already defined as Guid in the IdentityUser<Guid> base class.

        public string? FullName { get; set; }   // You might want to add a setter here if you want to modify it.
        public string? RefreshToken { get; set; }
        public string? OAuthProvider { get; set; }
        public string? OAuthId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public virtual UserProfile UserProfile { get; set; } // UserProfile is not optional

        public virtual Artist Artist { get; set; } // Artist is optional

        // Collection navigation property for Comments
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<CommentLikes> CommentLikes { get; set; }


        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();

    }


}
