using Microsoft.AspNetCore.Mvc;
using web.Models.Entities;
using web.Services;

namespace web.Controllers;

public class NewsController : Controller
{
    public IActionResult Index()
    {
        var items = InMemoryData.NewsItems
            .Where(n => n.FacultyId == 1) 
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

        return View(items);
    }
}
