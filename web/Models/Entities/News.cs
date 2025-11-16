namespace web.Models.Entities
{
    public class News
    {
        public int Id { get; set; }

        public string Category { get; set; } = "";

        public string Title { get; set; } = "";

        public string Content { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public int FacultyId { get; set; }

        public string? LinkText { get; set; }
        public string? LinkUrl { get; set; }
    }
}
