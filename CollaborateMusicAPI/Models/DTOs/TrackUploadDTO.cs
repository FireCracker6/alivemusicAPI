namespace ALIVEMusicAPI.Models.DTOs;

public class TrackUploadDTO
{
    public IFormFile TrackFile { get; set; } = null!;
    public string TrackName { get; set; } = null!;
    public TimeSpan? Duration { get; set; }
    public int? TrackNumber { get; set; }
    public string? Lyrics { get; set; }
    public int? AlbumID { get; set; }
    public int ArtistID { get; set; }
    public Guid? JobID { get; set; }
}
