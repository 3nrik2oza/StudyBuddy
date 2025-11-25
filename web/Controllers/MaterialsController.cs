using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models.Entities;
using web.Models.ViewModels;

namespace web.Controllers;

public class MaterialListItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string SubjectName { get; set; } = "";
    public string Type { get; set; } = "";
    public string? Url { get; set; }
}
[Authorize]
public class MaterialsController : Controller
{
    private readonly StudyBuddyDbContext _context;

    public MaterialsController(StudyBuddyDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int? subjectId, string? search)
    {
        var query = _context.Materials
            .Include(m => m.Subject)
            .AsQueryable();

        if (subjectId.HasValue)
        {
            query = query.Where(m => m.SubjectId == subjectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(m =>
                m.Title.ToLower().Contains(searchLower));
        }

        var items = query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MaterialListItemVM
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                SubjectName = m.Subject != null ? m.Subject.Name : "Neznan predmet",
                Type = m.Type,
                Url = m.Url
            })
            .ToList();

        ViewBag.Subjects = _context.Subjects.ToList();
        ViewBag.SelectedSubjectId = subjectId;
        ViewBag.Search = search;

        return View(items);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Subjects = _context.Subjects.ToList();
        return View(new MaterialCreateVM());
    }

    [HttpPost]
    public IActionResult Create(MaterialCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = _context.Subjects.ToList();
            return View(vm);
        }

        var newId = _context.Materials.Any()
            ? _context.Materials.Max(m => m.Id) + 1
            : 1;

        var entity = new Material
        {
            Id           = newId,          
            Title        = vm.Title,
            Description  = vm.Description,
            Type         = vm.Type,
            Url          = vm.Type == "link" ? vm.Url : null,
            SubjectId    = vm.SubjectId,
            FacultyId    = 1,          // za sada FRI
            AuthorUserId = "demo",     // kasnije iz Identity-ja
            CreatedAt    = DateTime.UtcNow
        };

        _context.Materials.Add(entity);
        _context.SaveChanges();

        TempData["ok"] = "Gradivo je uspješno dodano.";
        return RedirectToAction(nameof(Index));
    }

}
