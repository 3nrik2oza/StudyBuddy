using System;
using System.Text.Json.Serialization;

namespace web.Models.Entities
{
    public class ForumReply
    {
        public int Id { get; set; }

        public int ForumThreadId { get; set; }  
        [JsonIgnore]   
        public ForumThread ForumThread { get; set; } = null!;
        public string AuthorUserId { get; set; } = "";
        public string AuthorName { get; set; } = "";

        public string Content { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}
