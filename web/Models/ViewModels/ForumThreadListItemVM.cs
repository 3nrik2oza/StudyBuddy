namespace web.Models.ViewModels;

public class ForumThreadListItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string ContentPreview { get; set; } = "";
    public string Category { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int RepliesCount { get; set; }
}
