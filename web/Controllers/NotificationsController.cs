using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models;

namespace web.Controllers;

public class NotificationGroupVM
{
    public string Title { get; set; } = "";
    public string LastMessage { get; set; } = "";
    public string? Link { get; set; }
    public int Count { get; set; }
    public DateTime LastCreatedAt { get; set; }
    public int Id { get; set; }
}

[Authorize]
public class NotificationsController : Controller
{
    private readonly StudyBuddyDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationsController(
        StudyBuddyDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var meId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(meId))
            return Forbid();

        var notifications = await _context.Notifications
            .Where(n => n.UserId == meId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        var grouped = GroupNotifications(notifications);

        return View(grouped);
    }

    [HttpGet]
    public async Task<IActionResult> Dropdown()
    {
        var meId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(meId))
            return Unauthorized();

        var notifications = await _context.Notifications
            .Where(n => n.UserId == meId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Take(20)
            .ToListAsync();

        var grouped = GroupNotifications(notifications)
            .Take(5)
            .Select(n => new
            {
                id = n.Id,
                title = n.Title,
                message = n.LastMessage,
                link = n.Link,
                count = n.Count,
                createdAt = n.LastCreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm")
            })
            .ToList();

        var unreadCount = await _context.Notifications
            .CountAsync(n => n.UserId == meId && !n.IsRead);

        return Json(new
        {
            unreadCount,
            items = grouped
        });
    }

    private static List<NotificationGroupVM> GroupNotifications(List<Models.Entities.Notification> notifications)
    {
        return notifications
            .GroupBy(n => n.Link ?? "")
            .Select(g =>
            {
                var latest = g.OrderByDescending(x => x.CreatedAt).First();

                return new NotificationGroupVM
                {
                    Id = latest.Id,
                    Title = latest.Title,
                    LastMessage = latest.Message,
                    Link = latest.Link,
                    Count = g.Count(x => !x.IsRead),
                    LastCreatedAt = latest.CreatedAt
                };
            })
            .OrderByDescending(x => x.LastCreatedAt)
            .ToList();
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var meId = _userManager.GetUserId(User);

        if (string.IsNullOrWhiteSpace(meId))
            return Unauthorized();

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == meId);

        if (notification == null)
            return NotFound();

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return Redirect(notification.Link ?? "/Notifications");
    }
}