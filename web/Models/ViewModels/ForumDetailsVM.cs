using web.Models.Entities;

namespace web.Models.ViewModels;

public class ForumDetailsVM
{
    public ForumThread Thread { get; set; } = null!;
    public List<ForumReply> Replies { get; set; } = new();
    public ForumReplyCreateVM NewReply { get; set; } = new();
    public string? CurrentUserId { get; set; }
}
