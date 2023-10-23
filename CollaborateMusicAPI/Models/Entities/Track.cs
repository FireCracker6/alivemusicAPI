namespace CollaborateMusicAPI.Models.Entities;

public class Track
{
    public int TrackID { get; set; }
    public string TrackName { get; set; } = null!;
    public TimeSpan Duration { get; set; }
    public int TrackNumber { get; set; }
    public string FilePath { get; set; } = null!;   
    public string Lyrics { get; set; } = null!;

    public int AlbumID { get; set; }
    public virtual Album Album { get; set; } = null!;
}
