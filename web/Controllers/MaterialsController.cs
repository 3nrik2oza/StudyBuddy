using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public MaterialsController(StudyBuddyDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _env = env;
    }

    public IActionResult Index(int? subjectId, string? search)
    {
        var query = _context.Materials
            .Include(m => m.Subject)
            .AsQueryable();

        if (subjectId.HasValue)
            query = query.Where(m => m.SubjectId == subjectId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(m => m.Title.ToLower().Contains(s));
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MaterialCreateVM vm)
    {
        ViewBag.Subjects = _context.Subjects.ToList();

        var type = (vm.Type ?? "").Trim().ToLowerInvariant();

        if (type != "link" && type != "file")
            ModelState.AddModelError(nameof(vm.Type), "Invalid type.");

        if (type == "link")
        {
            if (string.IsNullOrWhiteSpace(vm.Url))
                ModelState.AddModelError(nameof(vm.Url), "Please enter a URL.");
        }
        else if (type == "file")
        {
            if (vm.UploadFile == null || vm.UploadFile.Length == 0)
                ModelState.AddModelError(nameof(vm.UploadFile), "Please upload a file.");
        }

        if (!ModelState.IsValid)
            return View(vm);

        string? storedPath = null;

        if (type == "file")
        {
            var maxBytes = 15 * 1024 * 1024;
            if (vm.UploadFile!.Length > maxBytes)
            {
                ModelState.AddModelError(nameof(vm.UploadFile), "File is too large (max 15MB).");
                return View(vm);
            }

            var ext = Path.GetExtension(vm.UploadFile.FileName).ToLowerInvariant();
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".txt", ".zip", ".png", ".jpg", ".jpeg"
            };

            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError(nameof(vm.UploadFile), "Unsupported file type.");
                return View(vm);
            }

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "materials");
            Directory.CreateDirectory(uploadsDir);

            var safeName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsDir, safeName);

            await using (var stream = System.IO.File.Create(fullPath))
            {
                await vm.UploadFile.CopyToAsync(stream);
            }

            storedPath = $"/uploads/materials/{safeName}";
        }

        var newId = _context.Materials.Any()
            ? _context.Materials.Max(m => m.Id) + 1
            : 1;

        var userId = _userManager.GetUserId(User);

        var entity = new Material
        {
            Id = newId,
            Title = vm.Title,
            Description = vm.Description,
            Type = type,
            Url = type == "link" ? vm.Url?.Trim() : storedPath,
            SubjectId = vm.SubjectId,
            FacultyId = 1,
            AuthorUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Materials.Add(entity);
        await _context.SaveChangesAsync();

        TempData["ok"] = "The material has been successfully added.";
        return RedirectToAction(nameof(Index));
    }
}
