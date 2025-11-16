using Microsoft.AspNetCore.Mvc;
using web.Models.Entities;
using web.Models.ViewModels;
using web.Services;

namespace web.Controllers;

public class StudyPostListItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public DateTime StartAt { get; set; }
    public string Location { get; set; } = "";
    public bool IsOnline { get; set; }
}

public class StudyPostsController : Controller
{
    public IActionResult Index(int? subjectId, DateTime? from)
    {
        var query = InMemoryData.StudyPosts.AsQueryable();

        var minDate = from ?? DateTime.Today;
        query = query.Where(p => p.StartAt >= minDate);

        if (subjectId.HasValue)
        {
            query = query.Where(p => p.SubjectId == subjectId.Value);
        }

        var items = query
            .OrderBy(p => p.StartAt)
            .AsEnumerable()
            .Select(p =>
            {
                var subject = InMemoryData.Subjects.FirstOrDefault(s => s.Id == p.SubjectId);
                var subjectName = subject != null ? subject.Name : "Neznan predmet";

                return new StudyPostListItemVM
                {
                    Id = p.Id,
                    Title = p.Title,
                    SubjectName = subjectName,
                    StartAt = p.StartAt,
                    Location = p.Location,
                    IsOnline = p.IsOnline
                };
            })
            .ToList();

        ViewBag.Subjects = InMemoryData.Subjects;
        ViewBag.From = minDate.ToString("yyyy-MM-dd");

        return View(items);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Subjects = InMemoryData.Subjects;
        // default: danas + 1h
        var vm = new StudyPostCreateVM
        {
            StartAt = DateTime.Now.AddHours(1)
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult Create(StudyPostCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = InMemoryData.Subjects;
            return View(vm);
        }

        var newId = InMemoryData.StudyPosts.Any()
            ? InMemoryData.StudyPosts.Max(p => p.Id) + 1
            : 1;

        var entity = new StudyPost
        {
            Id = newId,
            Title = vm.Title,
            SubjectId = vm.SubjectId,
            StartAt = vm.StartAt,
            Location = vm.Location,
            IsOnline = vm.IsOnline,
            FacultyId = 1,         // za sada hard-code
            AuthorUserId = "demo", // kasnije iz logiranog usera
            CreatedAt = DateTime.UtcNow
        };

        InMemoryData.StudyPosts.Add(entity);
        TempData["ok"] = "Termin je uspješno kreiran.";

        return RedirectToAction(nameof(Index));
    }
}
