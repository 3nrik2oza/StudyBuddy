using System.ComponentModel.DataAnnotations;
using web.Models.Entities;

namespace web.Models.ViewModels;

public class TutorRequestCreateVM
{
    public string TutorUserId { get; set; } = "";

    [Required]
    public int SubjectId { get; set; }

    [Required]
    [StringLength(800)]
    public string Message { get; set; } = "";

    [StringLength(200)]
    public string? PreferredTime { get; set; }

    public bool IsOnline { get; set; }
}

public class TutorRequestListItemVM
{
    public int Id { get; set; }
    public string StudentName { get; set; } = "";
    public string StudentEmail { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public string Message { get; set; } = "";
    public string? PreferredTime { get; set; }
    public bool IsOnline { get; set; }
    public TutorRequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
