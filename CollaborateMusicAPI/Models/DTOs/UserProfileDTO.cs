namespace ALIVEMusicAPI.Models.DTOs;

public class UserProfileDTO
{
    public int UserProfileID { get; set; }
    public string FullName { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfilePicturePath { get; set; }
    public IFormFile? ProfilePic { get; set; }
    public string? Location { get; set; }
    public string? WebsiteURL { get; set; }
    public Guid UserID { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureURL { get; set; }
    //public string? ArtistName { get; set; }
    //public string? ArtistBio { get; set; }
    //public string? ArtistLocation { get; set; }
    //public string? ArtistWebsiteURL { get; set; }
    //public string? ArtistProfilePicturePath { get; set; }
    //public string? ArtistProfilePictureURL { get; set; }
    //public string? ArtistGenre { get; set; }
    //public string? ArtistType { get; set; }
    //public string? ArtistSocialMediaURL { get; set; }
    //public string? ArtistSocialMediaHandle { get; set; }
    //public string? ArtistSpotifyURL { get; set; }
    //public string? ArtistAppleMusicURL { get; set; }
    //public string? ArtistSoundCloudURL { get; set; }
    //public string? ArtistYouTubeURL { get; set; }
    //public string? ArtistBandcampURL { get; set; }
    //public string? ArtistInstagramURL { get; set; }
    //public string? ArtistTwitterURL { get; set; }
    //public string? ArtistFacebookURL { get; set; }
    //public string? ArtistLinkedInURL { get; set; }
    //public string? ArtistTikTokURL { get; set; }
    //public string? ArtistSnapchatURL { get; set; }
    //public string? ArtistPinterestURL { get; set; }
    //public string? ArtistTumblrURL { get; set; }
    //public string? ArtistRedditURL { get; set; }
    //public string? ArtistTwitchURL { get; set; }
    //public string? ArtistPeriscopeURL { get; set; }
    //public string? ArtistVimeoURL { get; set; }
    //public string? ArtistFlickrURL { get;}
}
