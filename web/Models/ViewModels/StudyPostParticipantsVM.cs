using System;
using System.Collections.Generic;

namespace web.Models.ViewModels
{
    public class StudyPostParticipantsVM
    {
        public int StudyPostId { get; set; }
        public string Title { get; set; } = "";
        public int MaxParticipants { get; set; }
        public int Count { get; set; }
        public List<StudyPostParticipantItemVM> Participants { get; set; } = new();
    }

    public class StudyPostParticipantItemVM
    {
        public int ParticipantId { get; set; }
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime JoinedAt { get; set; }
        public bool CanRemove { get; set; }
    }
}
