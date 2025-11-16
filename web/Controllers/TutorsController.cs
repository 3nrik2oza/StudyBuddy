using Microsoft.AspNetCore.Mvc;
using web.Data;
using web.Models.Entities;

namespace web.Controllers;

public class TutorListItemVM
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int HelpPoints { get; set; }
    public string FacultyName { get; set; } = "";
    public List<string> Subjects { get; set; } = new();
}

public class TutorsController : Controller
{
    private readonly StudyBuddyDbContext _context;

    public TutorsController(StudyBuddyDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int? subjectId)
    {
        var tutorsQuery = _context.Tutors
            .Where(t => t.FacultyId == 1);

        var tutors = tutorsQuery.ToList();

        var tutorSubjectLookup = _context.TutorSubjects
            .GroupBy(ts => ts.UserId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.SubjectId).ToList()
            );

        var faculty = _context.Faculties.FirstOrDefault(f => f.Id == 1);
        var facultyName = faculty != null ? faculty.Name : "Unknown faculty";

        var items = new List<TutorListItemVM>();

        foreach (var tutor in tutors)
        {
            if (!tutorSubjectLookup.TryGetValue(tutor.Id, out var subjectIds))
            {
                subjectIds = new List<int>();
            }

            if (subjectId.HasValue && !subjectIds.Contains(subjectId.Value))
            {
                continue;
            }

            var subjectNames = _context.Subjects
                .Where(s => subjectIds.Contains(s.Id))
                .Select(s => s.Name)
                .ToList();

            items.Add(new TutorListItemVM
            {
                Id = tutor.Id,
                Name = tutor.Name,
                HelpPoints = tutor.HelpPoints,
                FacultyName = facultyName,
                Subjects = subjectNames
            });
        }

        items = items
            .OrderByDescending(t => t.HelpPoints)
            .ToList();

        var topTutors = items.Take(3).ToList();
        ViewBag.TopTutors = topTutors;

        ViewBag.Subjects = _context.Subjects.ToList();
        ViewBag.SelectedSubjectId = subjectId;

        return View(items);
    }

    [HttpPost]
    public IActionResult GiveHelpPoint(string tutorId, int? subjectId)
    {
        var tutor = _context.Tutors.FirstOrDefault(t => t.Id == tutorId);
        if (tutor != null)
        {
            tutor.HelpPoints += 1;
            _context.SaveChanges();   
            TempData["ok"] = "Help point has been added.";
        }

        return RedirectToAction(nameof(Index), new { subjectId = subjectId });
    }
}
