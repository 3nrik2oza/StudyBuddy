using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models;
using web.Models.Entities;

namespace web.Controllers;

[Authorize]
public class BookmarksController : Controller
{
    private readonly StudyBuddyDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookmarksController(StudyBuddyDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int type, int entityId, string? returnUrl)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return Forbid();

        if (!Enum.IsDefined(typeof(BookmarkType), type)) return BadRequest();
        var t = (BookmarkType)type;

        var existing = await _context.Bookmarks
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Type == t && x.EntityId == entityId);

        if (existing != null)
        {
            _context.Bookmarks.Remove(existing);
            await _context.SaveChangesAsync();
            TempData["ToastMessage"] = "Removed from saved.";
        }
        else
        {
            _context.Bookmarks.Add(new Bookmark
            {
                UserId = userId,
                Type = t,
                EntityId = entityId,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            TempData["ToastMessage"] = "Saved.";
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> My()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return Forbid();

        var bookmarks = await _context.Bookmarks
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var materialIds = bookmarks.Where(x => x.Type == BookmarkType.Material).Select(x => x.EntityId).Distinct().ToList();
        var postIds = bookmarks.Where(x => x.Type == BookmarkType.StudyPost).Select(x => x.EntityId).Distinct().ToList();

        var materials = await _context.Materials
            .Include(m => m.Subject)
            .Where(m => materialIds.Contains(m.Id))
            .Select(m => new
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                SubjectName = m.Subject != null ? m.Subject.Name : "",
                Type = m.Type,
                Url = m.Url,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();

        var posts = await _context.StudyPosts
            .Include(p => p.Subject)
            .Where(p => postIds.Contains(p.Id))
            .Select(p => new
            {
                Id = p.Id,
                Title = p.Title,
                SubjectName = p.Subject != null ? p.Subject.Name : "",
                StartAt = p.StartAt,
                Location = p.Location,
                IsOnline = p.IsOnline
            })
            .ToListAsync();

        ViewBag.Materials = materials;
        ViewBag.StudyPosts = posts;

        return View();
    }
}
