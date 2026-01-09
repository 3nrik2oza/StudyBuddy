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

    // ✅ JSON endpoint: subjects by faculty (used by Index + Create)
    [HttpGet]
    public async Task<IActionResult> SubjectsByFaculty(int facultyId)
    {
        var subjects = await _context.Subjects
            .Where(s => s.FacultyId == facultyId)
            .OrderBy(s => s.Name)
            .Select(s => new { id = s.Id, name = s.Name })
            .ToListAsync();

        return Json(subjects);
    }

    public async Task<IActionResult> Index(int? facultyId, int? subjectId, string? search)
    {
        // ✅ default faculty = logged in user's faculty
        var me = await _userManager.GetUserAsync(User);
        if (!facultyId.HasValue && me != null && me.FacultyId != 0)
            facultyId = me.FacultyId;

        var query = _context.Materials
            .Include(m => m.Subject)
            .AsQueryable();

        if (facultyId.HasValue)
            query = query.Where(m => m.FacultyId == facultyId.Value);

        if (subjectId.HasValue)
            query = query.Where(m => m.SubjectId == subjectId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(m => m.Title.ToLower().Contains(s));
        }

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MaterialListItemVM
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                SubjectName = m.Subject != null ? m.Subject.Name : "Unknown subject",
                Type = m.Type,
                Url = m.Url
            })
            .ToListAsync();

        ViewBag.Faculties = await _context.Faculties.OrderBy(f => f.Name).ToListAsync();

        // ✅ subjects dropdown only for selected faculty
        ViewBag.Subjects = facultyId.HasValue
            ? await _context.Subjects.Where(s => s.FacultyId == facultyId.Value).OrderBy(s => s.Name).ToListAsync()
            : new List<Subject>();

        ViewBag.SelectedFacultyId = facultyId;
        ViewBag.SelectedSubjectId = subjectId;
        ViewBag.Search = search ?? "";

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Faculties = await _context.Faculties.OrderBy(f => f.Name).ToListAsync();

        // default faculty = current user faculty (nice UX)
        var me = await _userManager.GetUserAsync(User);
        var defaultFacultyId = me?.FacultyId;

        ViewBag.DefaultFacultyId = defaultFacultyId;
        ViewBag.Subjects = defaultFacultyId.HasValue
            ? await _context.Subjects.Where(s => s.FacultyId == defaultFacultyId.Value).OrderBy(s => s.Name).ToListAsync()
            : new List<Subject>();

        return View(new MaterialCreateVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MaterialCreateVM vm, int? facultyId)
    {
        ViewBag.Faculties = await _context.Faculties.OrderBy(f => f.Name).ToListAsync();

        // subjects depend on selected faculty (fallback user faculty)
        var me = await _userManager.GetUserAsync(User);
        var resolvedFacultyId = facultyId ?? me?.FacultyId;

        ViewBag.DefaultFacultyId = resolvedFacultyId;
        ViewBag.Subjects = resolvedFacultyId.HasValue
            ? await _context.Subjects.Where(s => s.FacultyId == resolvedFacultyId.Value).OrderBy(s => s.Name).ToListAsync()
            : new List<Subject>();

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

        // ✅ validate subject exists
        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == vm.SubjectId);
        if (subject == null)
            ModelState.AddModelError(nameof(vm.SubjectId), "Please select a valid subject.");

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

        // ✅ FacultyId comes from subject (single source of truth)
        var entity = new Material
        {
            Id = newId,
            Title = vm.Title,
            Description = vm.Description,
            Type = type,
            Url = type == "link" ? vm.Url?.Trim() : storedPath,
            SubjectId = vm.SubjectId,
            FacultyId = subject!.FacultyId,
            AuthorUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Materials.Add(entity);
        await _context.SaveChangesAsync();

        TempData["ok"] = "The material has been successfully added.";
        return RedirectToAction(nameof(Index), new { facultyId = entity.FacultyId, subjectId = entity.SubjectId });
    }
}
