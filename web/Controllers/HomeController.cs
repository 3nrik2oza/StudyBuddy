using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models;
using web.Models.Entities;

namespace web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly StudyBuddyDbContext _context;

    public HomeController(ILogger<HomeController> logger, StudyBuddyDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var tutorsQuery = _context.Tutors
            .Include(t => t.Faculty)
            .Include(t => t.TutorSubjects)
                .ThenInclude(ts => ts.Subject)
            .Where(t => t.FacultyId == 1);

        var topTutors = tutorsQuery
            .OrderByDescending(t => t.HelpPoints)
            .Take(3)
            .ToList();

        var topTutorItems = topTutors.Select(t => new TutorListItemVM
        {
            Id = t.Id,
            Name = t.Name,
            HelpPoints = t.HelpPoints,
            FacultyName = t.Faculty != null ? t.Faculty.Name : "",
            Subjects = t.TutorSubjects
                .Where(ts => ts.Subject != null)
                .Select(ts => ts.Subject!.Name)
                .ToList()
        }).ToList();

        ViewBag.TopTutors = topTutorItems;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
