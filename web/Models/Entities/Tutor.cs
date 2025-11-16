namespace web.Models.Entities;

public class Tutor
{
    public string Id { get; set; } = "";   
    public string Name { get; set; } = "";
    public int FacultyId { get; set; }
    public Faculty? Faculty { get; set; }
    public int HelpPoints { get; set; }

    public ICollection<TutorSubject> TutorSubjects { get; set; } = new List<TutorSubject>();
}
