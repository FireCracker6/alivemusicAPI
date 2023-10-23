namespace CollaborateMusicAPI.Models.Entities;

public class Album
{
    public int AlbumID { get; set; }
    public string AlbumName { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public string? CoverImagePath { get; set; }
    public string? Genre { get; set; }
    public string? Description { get; set; }
    public int? TotalTracks { get; set; }

    public int ArtistID { get; set; }
    public virtual Artist Artist { get; set; } = null!;
}
