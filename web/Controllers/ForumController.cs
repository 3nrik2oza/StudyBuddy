using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models;
using web.Models.Entities;
using web.Models.ViewModels;

namespace web.Controllers;

[Authorize]
public class ForumController : Controller
{
    private readonly StudyBuddyDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ForumController(StudyBuddyDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // LISTA THREAD-OVA
    public IActionResult Index(int? subjectId, string? category, string? search)
    {
        var threads = _context.ForumThreads.AsQueryable();

        if (subjectId.HasValue)
            threads = threads.Where(t => t.SubjectId == subjectId.Value);

        if (!string.IsNullOrWhiteSpace(category))
        {
            var categoryLower = category.ToLower();
            threads = threads.Where(t => t.Category.ToLower() == categoryLower);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            threads = threads.Where(t =>
                t.Title.ToLower().Contains(searchLower) ||
                t.Content.ToLower().Contains(searchLower));
        }

        var subjectLookup = _context.Subjects.ToDictionary(s => s.Id, s => s.Name);

        var items = threads
            .OrderByDescending(t => t.CreatedAt)
            .AsEnumerable()
            .Select(t => new ForumThreadListItemVM
            {
                Id = t.Id,
                Title = t.Title,
                ContentPreview = t.Content.Length > 140 ? t.Content.Substring(0, 140) + "..." : t.Content,
                Category = t.Category,
                SubjectName = subjectLookup.TryGetValue(t.SubjectId, out var name) ? name : "",
                AuthorName = t.AuthorName,
                CreatedAt = t.CreatedAt,
                RepliesCount = t.RepliesCount
            })
            .ToList();

        ViewBag.Subjects = _context.Subjects.ToList();
        ViewBag.SelectedSubjectId = subjectId;
        ViewBag.SelectedCategory = category;
        ViewBag.Search = search;

        return View(items);
    }

    // CREATE THREAD (GET)
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Subjects = _context.Subjects.ToList();
        ViewBag.Categories = new List<string> { "Study group", "Materials", "Help" };

        return View(new ForumThreadCreateVM());
    }

    // CREATE THREAD (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ForumThreadCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = _context.Subjects.ToList();
            ViewBag.Categories = new List<string> { "Study group", "Materials", "Help" };
            return View(vm);
        }

        var newId = _context.ForumThreads.Any()
            ? _context.ForumThreads.Max(t => t.Id) + 1
            : 1;

        var appUser = await _userManager.GetUserAsync(User);

        var displayName =
            !string.IsNullOrWhiteSpace(appUser?.Name)
                ? appUser!.Name
                : (appUser?.Email ?? _userManager.GetUserName(User) ?? "Unknown");

        var entity = new ForumThread
        {
            Id = newId,
            Title = vm.Title,
            Content = vm.Content,
            Category = vm.Category,
            SubjectId = vm.SubjectId,
            FacultyId = 1,
            AuthorUserId = appUser?.Id ?? "",
            AuthorName = displayName,
            CreatedAt = DateTime.UtcNow,
            RepliesCount = 0
        };

        _context.ForumThreads.Add(entity);
        await _context.SaveChangesAsync();

        TempData["ok"] = "The thread has been successfully added..";
        return RedirectToAction(nameof(Index));
    }

    // DETAILS (THREAD + REPLIES)
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var thread = await _context.ForumThreads.FirstOrDefaultAsync(t => t.Id == id);
        if (thread == null) return NotFound();

        var replies = await _context.ForumReplies
            .Where(r => r.ForumThreadId == id)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

        var vm = new ForumDetailsVM
        {
            Thread = thread,
            Replies = replies,
            NewReply = new ForumReplyCreateVM { ForumThreadId = id },
            CurrentUserId = _userManager.GetUserId(User)
        };

        return View(vm);
    }

    // ADD REPLY
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReply([Bind(Prefix = "NewReply")] ForumReplyCreateVM vm)
    {
        var userId = _userManager.GetUserId(User);

        if (!ModelState.IsValid)
        {
            var thread = await _context.ForumThreads.FirstOrDefaultAsync(t => t.Id == vm.ForumThreadId);
            if (thread == null) return NotFound();

            var replies = await _context.ForumReplies
                .Where(r => r.ForumThreadId == vm.ForumThreadId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

            var detailsVm = new ForumDetailsVM
            {
                Thread = thread,
                Replies = replies,
                NewReply = vm,
                CurrentUserId = userId
            };

            return View("Details", detailsVm);
        }

        var appUser = await _userManager.GetUserAsync(User);

        var displayName =
            !string.IsNullOrWhiteSpace(appUser?.Name)
                ? appUser!.Name
                : (appUser?.Email ?? _userManager.GetUserName(User) ?? "Unknown");

        var newId = _context.ForumReplies.Any()
            ? _context.ForumReplies.Max(r => r.Id) + 1
            : 1;

        var reply = new ForumReply
        {
            Id = newId,
            ForumThreadId = vm.ForumThreadId,
            Content = vm.Content,
            AuthorUserId = appUser?.Id ?? "",
            AuthorName = displayName,
            CreatedAt = DateTime.UtcNow
        };

        _context.ForumReplies.Add(reply);

        var threadToUpdate = await _context.ForumThreads.FirstOrDefaultAsync(t => t.Id == vm.ForumThreadId);
        if (threadToUpdate != null)
            threadToUpdate.RepliesCount += 1;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = vm.ForumThreadId });
    }

    // DELETE THREAD (samo autor)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteThread(int id)
    {
        var thread = await _context.ForumThreads.FirstOrDefaultAsync(t => t.Id == id);
        if (thread == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        if (thread.AuthorUserId != userId)
            return Forbid();

        var replies = await _context.ForumReplies
            .Where(r => r.ForumThreadId == id)
            .ToListAsync();

        _context.ForumReplies.RemoveRange(replies);

        _context.ForumThreads.Remove(thread);
        await _context.SaveChangesAsync();

        TempData["ok"] = "The thread has been successfully deleted.";
        return RedirectToAction(nameof(Index));
    }

    // DELETE REPLY (samo autor)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReply(int id, int threadId)
    {
        var reply = await _context.ForumReplies.FirstOrDefaultAsync(r => r.Id == id);
        if (reply == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        if (reply.AuthorUserId != userId)
            return Forbid();

        _context.ForumReplies.Remove(reply);

        var thread = await _context.ForumThreads.FirstOrDefaultAsync(t => t.Id == threadId);
        if (thread != null && thread.RepliesCount > 0)
            thread.RepliesCount -= 1;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = threadId });
    }
}
