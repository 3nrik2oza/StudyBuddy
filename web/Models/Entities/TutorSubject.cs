namespace web.Models.Entities;

public class TutorSubject
{
    public string UserId { get; set; } = "";
    public Tutor? Tutor { get; set; }
    //public ApplicationUser? User { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
}
