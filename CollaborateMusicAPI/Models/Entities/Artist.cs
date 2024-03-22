using System.ComponentModel.DataAnnotations;
using ALIVEMusicAPI.Models.Entities;
using CollaborateMusicAPI.Contexts;

namespace CollaborateMusicAPI.Models.Entities;

public class Artist
{
    [Key]
    public int ArtistID { get; set; }
    public string ArtistName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public string? ArtistPicturePath { get; set; }
    public Guid UserID { get; set; } 

    // Collection navigation property for Comments
    public virtual ICollection<Comment>? Comments { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}

