namespace web.Models.Entities;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Code { get; set; }
    public string? Description { get; set; }

    public int FacultyId { get; set; }
    public Faculty? Faculty { get; set; }
}
