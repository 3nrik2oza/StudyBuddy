using System.ComponentModel.DataAnnotations;
using web.Models;

namespace web.Models.Entities;

public class TutorRequestMessage
{
    public int Id { get; set; }

    public int TutorRequestId { get; set; }
    public TutorRequest? TutorRequest { get; set; }

    [Required]
    public string SenderUserId { get; set; } = "";
    public ApplicationUser? SenderUser { get; set; }

    [Required, StringLength(2000)]
    public string Body { get; set; } = "";

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
