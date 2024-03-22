using ALIVEMusicAPI.Models.Entities;

namespace CollaborateMusicAPI.Models.Entities;

public class Track
{
    public int TrackID { get; set; }
    public Guid? JobID { get; set; }
    public string TrackName { get; set; } = null!;
    public TimeSpan? Duration { get; set; }
    public int? TrackNumber { get; set; }
  
    public string? Lyrics { get; set; }
    public string TrackFilePath { get; set; } = null!;

    public int? AlbumID { get; set; }
    public virtual Album? Album { get; set; } 


    public int ArtistID { get; set; }
    public virtual Artist Artist { get; set; } = null!;

    // Collection navigation property for Comments
    public virtual ICollection<Comment>? Comments { get; set; }

    public virtual ICollection<Likes> Likes { get; set; } = new List<Likes>();

}
