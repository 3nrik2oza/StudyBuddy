namespace web.Models.Entities
{
    public class ForumThread
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string Category { get; set; } = ""; 
        public int SubjectId { get; set; }
        public int FacultyId { get; set; }
        public string AuthorUserId { get; set; } = "";
        public string AuthorName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int RepliesCount { get; set; }
        public List<ForumReply> Replies { get; set; } = new();
    }
}
