using CollaborateMusicAPI.Models.Entities;

namespace ALIVEMusicAPI.Models.DTOs;

public class CommentsDTO
{
    public int CommentID { get; set; }
    public string? Content { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public int ArtistID { get; set; }
    public string? ArtistName { get; set; }
    public virtual Artist? Artist { get; set; }
    public Guid UserID { get; set; }
    public int? ParentCommentID { get; set; }
    public int? TrackID { get; set; }
    public string? UserName { get; set; } = null!;
    public string? UserPicturePath { get; set; }
    //public int Likes { get; set; }
    //public int Dislikes { get; set; }
    //public bool UserLiked { get; set; }
    //public bool UserDisliked { get; set; }
    //public int? ParentCommentID { get; set; }
    //public int? TrackID { get; set; }
    //public string? TrackName { get; set; }
    //public string? TrackPicturePath { get; set; }
    //public int? AlbumID { get; set; }
    //public string? AlbumName { get; set; }
    //public string? AlbumPicturePath { get; set; }
    //public int? PlaylistID { get; set; }
    //public string? PlaylistName { get; set; }
    //public string? PlaylistPicturePath { get; set; }
    //public int? PostID { get; set; }
    //public string? PostText { get; set; }
    //public string? PostPicturePath { get; set; }
    //public int? CommentCount { get; set; }
    //public int? LikeCount { get; set; }
    //public int? DislikeCount { get; set; }
    //public int? ShareCount { get; set; }
    //public int? RepostCount { get; set; }
    //public int? TrackCount { get; set; }
    //public int? AlbumCount { get; set; }
    //public int? PlaylistCount { get; set; }
    //public int? PostCount { get; set; }
    //public int? CommentLikeCount { get; set; }
    //public int? CommentDislikeCount { get; set; }
    //public int? CommentShareCount { get; set; }
    //public int? CommentRepostCount { get; set; }
    //public int? CommentCountCount { get; set; }
    //public int? CommentLikeCountCount { get; set; }
    //public int? CommentDislikeCountCount { get; set; }

    public List<CommentsDTO>? Replies { get; set; }

}
