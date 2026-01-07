using System;

namespace web.Models.Entities
{
    public class ForumReply
    {
        public int Id { get; set; }

        public int ForumThreadId { get; set; }     
        public ForumThread ForumThread { get; set; } = null!;
        public string AuthorUserId { get; set; } = "";
        public string AuthorName { get; set; } = "";

        public string Content { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}
