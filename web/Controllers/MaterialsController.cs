using Microsoft.AspNetCore.Mvc;
using web.Services;
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

public class MaterialsController : Controller
{
    public IActionResult Index(int? subjectId, string? search)
    {
        var query = InMemoryData.Materials.AsQueryable();

        if (subjectId.HasValue)
        {
            query = query.Where(m => m.SubjectId == subjectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(m =>
                m.Title.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var items = query
            .OrderByDescending(m => m.CreatedAt)
            .AsEnumerable()   
            .Select(m =>
            {
                var subject = InMemoryData.Subjects.FirstOrDefault(s => s.Id == m.SubjectId);
                var subjectName = subject != null ? subject.Name : "Neznan predmet";

                return new MaterialListItemVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    SubjectName = subjectName,
                    Type = m.Type,
                    Url = m.Url
                };
            })
            .ToList();


       ViewBag.Subjects = InMemoryData.Subjects;
        ViewBag.SelectedSubjectId = subjectId;
        ViewBag.Search = search;

        return View(items);

    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Subjects = InMemoryData.Subjects;
        return View(new MaterialCreateVM());
    }

    [HttpPost]
    public IActionResult Create(MaterialCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = InMemoryData.Subjects;
            return View(vm);
        }

        var newId = InMemoryData.Materials.Any()
            ? InMemoryData.Materials.Max(m => m.Id) + 1
            : 1;

        var entity = new Material
        {
            Id = newId,
            Title = vm.Title,
            Description = vm.Description,
            Type = vm.Type,
            Url = vm.Type == "link" ? vm.Url : null,
            SubjectId = vm.SubjectId,
            FacultyId = 1,
            AuthorUserId = "demo",
            CreatedAt = DateTime.UtcNow
        };

        InMemoryData.Materials.Add(entity);
        TempData["ok"] = "Gradivo je uspješno dodano.";

        return RedirectToAction(nameof(Index));
    }
}
