using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models;
using web.Models.Entities;
using web.Models.ViewModels;

namespace web.Controllers;

public class StudyPostListItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public DateTime StartAt { get; set; }
    public string Location { get; set; } = "";
    public bool IsOnline { get; set; }
    public int MaxParticipants { get; set; }
    public int ParticipantsCount { get; set; }
    public bool IsFull => ParticipantsCount >= MaxParticipants;
    public bool IsJoined { get; set; }
    public bool IsOwner { get; set; }
    public string OrganizerName { get; set; } = "";
}

[Authorize]
public class StudyPostsController : Controller
{
    private readonly StudyBuddyDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudyPostsController(StudyBuddyDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index(int? subjectId, DateTime? from)
    {
        DateTime minDateUtc;

        if (from.HasValue)
        {
            var localFrom = DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Local);
            minDateUtc = localFrom.ToUniversalTime();
        }
        else
        {
            var localNow = DateTime.Now;
            var localStartOfDay = new DateTime(localNow.Year, localNow.Month, localNow.Day, 0, 0, 0, DateTimeKind.Local);
            minDateUtc = localStartOfDay.ToUniversalTime();
        }

        var userId = _userManager.GetUserId(User) ?? "";

        var query = _context.StudyPosts
            .Include(p => p.Subject)
            .Where(p => p.StartAt >= minDateUtc);

        if (subjectId.HasValue)
            query = query.Where(p => p.SubjectId == subjectId.Value);

        var items = query
            .OrderBy(p => p.StartAt)
            .Select(p => new StudyPostListItemVM
            {
                Id = p.Id,
                Title = p.Title,
                SubjectName = p.Subject != null ? p.Subject.Name : "Neznan predmet",
                StartAt = p.StartAt,
                Location = p.Location,
                IsOnline = p.IsOnline,
                MaxParticipants = p.MaxParticipants,
                ParticipantsCount = _context.StudyPostParticipants.Count(x => x.StudyPostId == p.Id),
                IsJoined = userId != "" && _context.StudyPostParticipants.Any(x => x.StudyPostId == p.Id && x.UserId == userId),
                IsOwner = userId != "" && p.AuthorUserId == userId,
                OrganizerName = _context.Users
                    .Where(u => u.Id == p.AuthorUserId)
                    .Select(u => (u.Name != null && u.Name != "") ? u.Name : (u.Email ?? "Unknown"))
                    .FirstOrDefault() ?? "Unknown"
            })
            .ToList();

        ViewBag.Subjects = _context.Subjects.ToList();
        ViewBag.From = from.HasValue ? from.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");

        return View(items);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Subjects = _context.Subjects.ToList();

        var vm = new StudyPostCreateVM
        {
            StartAt = DateTime.Now.AddHours(1),
            MaxParticipants = 10
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(StudyPostCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = _context.Subjects.ToList();
            return View(vm);
        }

        var newId = _context.StudyPosts.Any()
            ? _context.StudyPosts.Max(p => p.Id) + 1
            : 1;

        var startAtUtc = DateTime.SpecifyKind(vm.StartAt, DateTimeKind.Local).ToUniversalTime();

        var userId = _userManager.GetUserId(User) ?? "";

        var entity = new StudyPost
        {
            Id = newId,
            Title = vm.Title,
            SubjectId = vm.SubjectId,
            StartAt = startAtUtc,
            Location = vm.Location,
            IsOnline = vm.IsOnline,
            FacultyId = 1,
            AuthorUserId = userId,
            CreatedAt = DateTime.UtcNow,
            MaxParticipants = vm.MaxParticipants
        };

        _context.StudyPosts.Add(entity);
        _context.SaveChanges();

        if (userId != "")
        {
            var already = _context.StudyPostParticipants.Any(x => x.StudyPostId == entity.Id && x.UserId == userId);
            if (!already)
            {
                _context.StudyPostParticipants.Add(new StudyPostParticipant
                {
                    StudyPostId = entity.Id,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow
                });
                _context.SaveChanges();
            }
        }

        TempData["ToastMessage"] = "Study session has been created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return Forbid();

        var post = await _context.StudyPosts.FirstOrDefaultAsync(x => x.Id == id);
        if (post == null) return NotFound();

        var already = await _context.StudyPostParticipants
            .AnyAsync(x => x.StudyPostId == id && x.UserId == userId);

        if (already)
        {
            TempData["ToastMessage"] = "You already joined this session.";
            return RedirectToAction(nameof(Index));
        }

        var currentCount = await _context.StudyPostParticipants
            .CountAsync(x => x.StudyPostId == id);

        if (currentCount >= post.MaxParticipants)
        {
            TempData["ToastMessage"] = "Session is full.";
            return RedirectToAction(nameof(Index));
        }

        _context.StudyPostParticipants.Add(new StudyPostParticipant
        {
            StudyPostId = id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        TempData["ToastMessage"] = "Joined successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Leave(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return Forbid();

        var entry = await _context.StudyPostParticipants
            .FirstOrDefaultAsync(x => x.StudyPostId == id && x.UserId == userId);

        if (entry != null)
        {
            _context.StudyPostParticipants.Remove(entry);
            await _context.SaveChangesAsync();
        }

        TempData["ToastMessage"] = "Left the session.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Participants(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return Forbid();

        var post = await _context.StudyPosts.FirstOrDefaultAsync(x => x.Id == id);
        if (post == null) return NotFound();
        if (post.AuthorUserId != userId) return Forbid();

        var participants = await _context.StudyPostParticipants
            .Where(x => x.StudyPostId == id)
            .OrderBy(x => x.JoinedAt)
            .ToListAsync();

        var userIds = participants.Select(x => x.UserId).Distinct().ToList();

        var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Name, u.Email })
            .ToListAsync();

        var lookup = users.ToDictionary(x => x.Id, x => x);

        var vm = new StudyPostParticipantsVM
        {
            StudyPostId = post.Id,
            Title = post.Title,
            MaxParticipants = post.MaxParticipants,
            Count = participants.Count,
            Participants = participants.Select(p =>
            {
                lookup.TryGetValue(p.UserId, out var u);
                return new StudyPostParticipantItemVM
                {
                    ParticipantId = p.Id,
                    UserId = p.UserId,
                    Name = (u?.Name != null && u.Name != "") ? u.Name : "",
                    Email = u?.Email ?? "",
                    JoinedAt = p.JoinedAt,
                    CanRemove = p.UserId != userId
                };
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveParticipant(int participantId, int postId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return Forbid();

        var post = await _context.StudyPosts.FirstOrDefaultAsync(x => x.Id == postId);
        if (post == null) return NotFound();
        if (post.AuthorUserId != userId) return Forbid();

        var participant = await _context.StudyPostParticipants.FirstOrDefaultAsync(x => x.Id == participantId);
        if (participant == null) return NotFound();
        if (participant.StudyPostId != postId) return BadRequest();
        if (participant.UserId == userId) return BadRequest();

        _context.StudyPostParticipants.Remove(participant);
        await _context.SaveChangesAsync();

        TempData["ToastMessage"] = "Participant removed.";
        return RedirectToAction(nameof(Participants), new { id = postId });
    }
}
