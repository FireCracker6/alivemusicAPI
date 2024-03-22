using ALIVEMusicAPI.Models.Entities;

namespace ALIVEMusicAPI.Models;

public class CommentClosure
{
    public int AncestorId { get; set; }
    public Comment Ancestor { get; set; } = null!;

    public int DescendantId { get; set; }
    public Comment Descendant { get; set; } = null!;

    public int Depth { get; set; }
}
