namespace web.Models.ViewModels;

using System.ComponentModel.DataAnnotations;

public class StudyPostCreateVM
{
    [Required]
    [Display(Name = "Title")]
    public string Title { get; set; } = "";

    [Required]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }

    [Required]
    [Display(Name = "Date and time")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime StartAt { get; set; }

    [Required]
    [Display(Name = "Location")]
    public string Location { get; set; } = "";

    [Display(Name = "Online session")]
    public bool IsOnline { get; set; }

    [Range(1, 100)]
    public int MaxParticipants { get; set; } = 10;
}
