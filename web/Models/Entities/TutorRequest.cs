using web.Models;

namespace web.Models.Entities;

public enum TutorRequestStatus
{
    Pending = 0,
    Accepted = 1,
    Declined = 2
}

public class TutorRequest
{
    public int Id { get; set; }

    public string StudentUserId { get; set; } = "";
    public ApplicationUser? StudentUser { get; set; }

    public string TutorUserId { get; set; } = "";
    public ApplicationUser? TutorUser { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public string Message { get; set; } = "";
    public string? PreferredTime { get; set; }
    public bool IsOnline { get; set; }

    public TutorRequestStatus Status { get; set; } = TutorRequestStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }

    public string? TutorResponseMessage { get; set; }
    public List<TutorRequestMessage> Messages { get; set; } = new();

}
