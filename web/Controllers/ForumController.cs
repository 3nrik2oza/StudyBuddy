using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models.Entities;

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
    private readonly StudyBuddyDbContext _context;

    public ForumController(StudyBuddyDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int? subjectId, string? category, string? search)
    {
        var threads = _context.ForumThreads.AsQueryable();

        if (subjectId.HasValue)
        {
            threads = threads.Where(t => t.SubjectId == subjectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            threads = threads.Where(t =>
                t.Category.ToLower() == category.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();

            threads = threads.Where(t =>
                t.Title.ToLower().Contains(searchLower) ||
                t.Content.ToLower().Contains(searchLower));
        }

        var subjectLookup = _context.Subjects
            .ToDictionary(s => s.Id, s => s.Name);

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
                SubjectName = subjectLookup.TryGetValue(t.SubjectId, out var name) ? name : "",
                AuthorName = t.AuthorName,
                CreatedAt = t.CreatedAt,
                RepliesCount = t.RepliesCount
            })
            .ToList();

        ViewBag.Subjects = _context.Subjects.ToList();
        ViewBag.SelectedSubjectId = subjectId;
        ViewBag.SelectedCategory = category;
        ViewBag.Search = search;

        return View(items);
    }
}
