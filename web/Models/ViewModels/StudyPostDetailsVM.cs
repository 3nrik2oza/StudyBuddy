namespace web.Models.ViewModels;

public class StudyPostDetailsVM
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public string OrganizerName { get; set; } = "";
    public string Location { get; set; } = "";
    public bool IsOnline { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MaxParticipants { get; set; }
    public int ParticipantsCount { get; set; }
    public bool IsFull => ParticipantsCount >= MaxParticipants;
    public bool IsJoined { get; set; }
    public bool IsOwner { get; set; }
    public List<StudyPostParticipantItemVM> Participants { get; set; } = new();
}