using web.Models;

namespace web.Models.Entities;

public enum BookmarkType
{
    Material = 1,
    StudyPost = 2
}

public class Bookmark
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";
    public ApplicationUser? User { get; set; }

    public BookmarkType Type { get; set; }

    public int EntityId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
