using System.ComponentModel.DataAnnotations;

namespace web.Models.ViewModels
{
    public class ForumReplyCreateVM
    {
        [Required]
        public int ForumThreadId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = "";
    }
}
