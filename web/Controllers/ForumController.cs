using Microsoft.AspNetCore.Mvc;
using System.Linq;
using web.Models.ViewModels;
using web.Services;

namespace web.Controllers;

public class ForumThreadListItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string ContentPreview { get; set; } = "";
    public string Category { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int RepliesCount { get; set; }
}

public class ForumController : Controller
{
    public IActionResult Index(int? subjectId, string? category, string? search)
    {
        var threads = InMemoryData.ForumThreads.AsQueryable();

        if (subjectId.HasValue)
        {
            threads = threads.Where(t => t.SubjectId == subjectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            threads = threads.Where(t =>
                t.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            threads = threads.Where(t =>
                t.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                t.Content.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var items = threads
            .OrderByDescending(t => t.CreatedAt)
            .AsEnumerable() 
            .Select(t => new ForumThreadListItemVM
            {
                Id = t.Id,
                Title = t.Title,
                ContentPreview = t.Content.Length > 140
                    ? t.Content.Substring(0, 140) + "..."
                    : t.Content,
                Category = t.Category,
                SubjectName = InMemoryData.Subjects
                    .FirstOrDefault(s => s.Id == t.SubjectId)?.Name ?? "",
                AuthorName = t.AuthorName,
                CreatedAt = t.CreatedAt,
                RepliesCount = t.RepliesCount
            })
            .ToList();

        ViewBag.Subjects = InMemoryData.Subjects;
        ViewBag.SelectedSubjectId = subjectId;
        ViewBag.SelectedCategory = category;
        ViewBag.Search = search;

        return View(items);
    }
}
