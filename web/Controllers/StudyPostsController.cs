using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models.Entities;
using web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using web.Models;

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
[Authorize]
public class StudyPostsController : Controller
{
    private readonly StudyBuddyDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

    public StudyPostsController(StudyBuddyDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index(int? subjectId, DateTime? from)
    {
        DateTime minDate;

        if (from.HasValue)
        {
            minDate = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
        }
        else
        {
            var nowUtc = DateTime.UtcNow;
            minDate = new DateTime(nowUtc.Year, nowUtc.Month, nowUtc.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        var query = _context.StudyPosts
            .Include(p => p.Subject)
            .Where(p => p.StartAt >= minDate);


        if (subjectId.HasValue)
        {
            query = query.Where(p => p.SubjectId == subjectId.Value);
        }

        var items = query
            .OrderBy(p => p.StartAt)
            .Select(p => new StudyPostListItemVM
            {
                Id          = p.Id,
                Title       = p.Title,
                SubjectName = p.Subject != null ? p.Subject.Name : "Neznan predmet",
                StartAt     = p.StartAt,
                Location    = p.Location,
                IsOnline    = p.IsOnline
            })
            .ToList();

        ViewBag.Subjects = _context.Subjects.ToList();
        ViewBag.From = minDate.ToString("yyyy-MM-dd");

        return View(items);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Subjects = _context.Subjects.ToList();

        var vm = new StudyPostCreateVM
        {
            StartAt = DateTime.UtcNow.AddHours(1)
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult Create(StudyPostCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = _context.Subjects.ToList();
            return View(vm);
        }

        var newId = _context.StudyPosts.Any()
            ? _context.StudyPosts.Max(p => p.Id) + 1
            : 1;

        var startAtUtc = DateTime.SpecifyKind(vm.StartAt, DateTimeKind.Utc);

        var userId = _userManager.GetUserId(User);

        var entity = new StudyPost
        {
            Id           = newId,
            Title        = vm.Title,
            SubjectId    = vm.SubjectId,
            StartAt      = startAtUtc,
            Location     = vm.Location,
            IsOnline     = vm.IsOnline,
            FacultyId    = 1,
            AuthorUserId = userId,
            CreatedAt    = DateTime.UtcNow
        };

        _context.StudyPosts.Add(entity);
        _context.SaveChanges();

        TempData["ok"] = "Termin je uspješno kreiran.";

        return RedirectToAction(nameof(Index));
    }
}
