namespace web.Models.Entities;

public class Material
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }

    public string Type { get; set; } = "link"; // "link" ili "file"
    public string? Url { get; set; }
    public string? FilePath { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public string AuthorUserId { get; set; } = "";
    //public ApplicationUser? Author { get; set; }

    public int FacultyId { get; set; }
    public Faculty? Faculty { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
