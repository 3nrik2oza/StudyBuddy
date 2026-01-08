namespace web.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class MaterialCreateVM
{
    [Required]
    [StringLength(120)]
    [Display(Name = "Title")]
    public string Title { get; set; } = "";

    [StringLength(500)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Type")]
    public string Type { get; set; } = "link"; 

    [Url]
    [Display(Name = "URL (if it is a link)")]
    public string? Url { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }
    public IFormFile? UploadFile { get; set; }

}
