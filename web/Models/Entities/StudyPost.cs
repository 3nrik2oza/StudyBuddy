namespace web.Models.Entities;

public class StudyPost
{
    public int Id { get; set; }
    public string Title { get; set; } = "";

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public string AuthorUserId { get; set; } = "";
    //public ApplicationUser? Author { get; set; }

    public string Location { get; set; } = "";
    public DateTime StartAt { get; set; }
    public bool IsOnline { get; set; }

    public int FacultyId { get; set; }
    public Faculty? Faculty { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
