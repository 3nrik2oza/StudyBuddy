using web.Models;

namespace web.Models.Entities;

public class StudyPostParticipant
{
    public int Id { get; set; }

    public int StudyPostId { get; set; }
    public StudyPost StudyPost { get; set; } = null!;

    public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
