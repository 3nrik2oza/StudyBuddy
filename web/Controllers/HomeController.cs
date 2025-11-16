using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using web.Models;
using web.Services;
using web.Models.Entities;

namespace web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
{
    var topTutors = InMemoryData.Tutors
        .OrderByDescending(t => t.HelpPoints)
        .Take(3)
        .Select(t => new TutorListItemVM
        {
            Name = t.Name,
            HelpPoints = t.HelpPoints,
            FacultyName = InMemoryData.Faculties
                .FirstOrDefault(f => f.Id == t.FacultyId)?.Name ?? "",
            Subjects = InMemoryData.TutorSubjects
                .Where(ts => ts.UserId == t.Id)
                .Join(
                    InMemoryData.Subjects,
                    ts => ts.SubjectId,
                    s => s.Id,
                    (ts, s) => s.Name
                )
                .ToList()
        })
        .ToList();

    ViewBag.TopTutors = topTutors;

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
