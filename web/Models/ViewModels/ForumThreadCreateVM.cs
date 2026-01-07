using System.ComponentModel.DataAnnotations;

namespace web.Models.ViewModels;

public class ForumThreadCreateVM
{
    [Required]
    [StringLength(120)]
    public string Title { get; set; } = "";

    [Required]
    [StringLength(4000)]
    public string Content { get; set; } = "";

    [Required]
    public string Category { get; set; } = "Help";

    [Required]
    public int SubjectId { get; set; }
}
