using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models;
using web.Models.Entities;
using web.Models.ViewModels;
using web.Services;

namespace web.Controllers;

[Authorize]
public class TutorListItemVM
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int HelpPoints { get; set; }
    public string FacultyName { get; set; } = "";
    public List<string> Subjects { get; set; } = new();
    public string? ProfilePicturePath { get; set; }
    public string? Email { get; set; }
}

[Authorize]
public class TutorsController : Controller
{
    private readonly StudyBuddyDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _email;

    public TutorsController(StudyBuddyDbContext context, UserManager<ApplicationUser> userManager, IEmailSender email)
    {
        _context = context;
        _userManager = userManager;
        _email = email;
    }

    public async Task<IActionResult> Index(int? subjectId, string? search)
    {
        var me = await _userManager.GetUserAsync(User);
        var myFacultyId = me?.FacultyId ?? 0;

        var tutorsQuery = _context.Tutors.AsQueryable();
        if (myFacultyId != 0)
            tutorsQuery = tutorsQuery.Where(t => t.FacultyId == myFacultyId);

        var tutors = await tutorsQuery.ToListAsync();

        var subjectsForFaculty = myFacultyId != 0
            ? await _context.Subjects.Where(s => s.FacultyId == myFacultyId).OrderBy(s => s.Name).ToListAsync()
            : await _context.Subjects.OrderBy(s => s.Name).ToListAsync();

        ViewBag.Subjects = subjectsForFaculty;
        ViewBag.SelectedSubjectId = subjectId;
        ViewBag.Search = search ?? "";

        var facultyName = "Unknown faculty";
        if (myFacultyId != 0)
            facultyName = await _context.Faculties.Where(f => f.Id == myFacultyId).Select(f => f.Name).FirstOrDefaultAsync() ?? "Unknown faculty";

        var tutorIds = tutors.Select(t => t.Id).ToList();

        var tutorSubjectLookup = await _context.TutorSubjects
            .Where(ts => tutorIds.Contains(ts.UserId))
            .GroupBy(ts => ts.UserId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.SubjectId).ToList());

        var userLookup = await _context.Users
            .Where(u => tutorIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email, u.ProfilePicturePath })
            .ToDictionaryAsync(x => x.Id, x => new { x.Email, x.ProfilePicturePath });

        var subjectNameById = subjectsForFaculty.ToDictionary(s => s.Id, s => s.Name);

        var items = new List<TutorListItemVM>();

        foreach (var tutor in tutors)
        {
            if (!tutorSubjectLookup.TryGetValue(tutor.Id, out var subjectIds))
                subjectIds = new List<int>();

            if (subjectId.HasValue && !subjectIds.Contains(subjectId.Value))
                continue;

            var subjectNames = subjectIds
                .Where(id => subjectNameById.ContainsKey(id))
                .Select(id => subjectNameById[id])
                .ToList();

            userLookup.TryGetValue(tutor.Id, out var u);

            items.Add(new TutorListItemVM
            {
                Id = tutor.Id,
                Name = tutor.Name,
                HelpPoints = tutor.HelpPoints,
                FacultyName = facultyName,
                Subjects = subjectNames,
                ProfilePicturePath = u?.ProfilePicturePath,
                Email = u?.Email
            });
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            items = items.Where(x =>
                    (!string.IsNullOrWhiteSpace(x.Name) && x.Name.ToLower().Contains(s)) ||
                    (!string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower().Contains(s))
                )
                .ToList();
        }

        items = items
            .OrderByDescending(t => t.HelpPoints)
            .ToList();

        ViewBag.TopTutors = items.Take(3).ToList();

        var meId = _userManager.GetUserId(User);
        ViewBag.IsCurrentUserTutor = !string.IsNullOrWhiteSpace(meId) && await _context.Tutors.AnyAsync(t => t.Id == meId);

        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GiveHelpPoint(string tutorId, int? subjectId, string? search)
    {
        var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.Id == tutorId);
        if (tutor != null)
        {
            tutor.HelpPoints += 1;
            await _context.SaveChangesAsync();
            TempData["ToastMessage"] = "Help point has been added.";
        }

        return RedirectToAction(nameof(Index), new { subjectId = subjectId, search = search });
    }

    [HttpGet]
    public async Task<IActionResult> Request(string tutorId)
    {
        var me = await _userManager.GetUserAsync(User);
        var myFacultyId = me?.FacultyId ?? 0;

        var tutorUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == tutorId);
        if (tutorUser == null) return NotFound();

        if (myFacultyId != 0)
        {
            var tutorFacultyId = await _context.Tutors.Where(t => t.Id == tutorId).Select(t => (int?)t.FacultyId).FirstOrDefaultAsync();
            if (tutorFacultyId.HasValue && tutorFacultyId.Value != myFacultyId)
                return Forbid();
        }

        ViewBag.TutorName = tutorUser.Name;

        ViewBag.Subjects = myFacultyId != 0
            ? await _context.Subjects.Where(s => s.FacultyId == myFacultyId).OrderBy(s => s.Name).ToListAsync()
            : await _context.Subjects.OrderBy(s => s.Name).ToListAsync();

        return View(new TutorRequestCreateVM
        {
            TutorUserId = tutorId,
            IsOnline = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Request(TutorRequestCreateVM vm)
    {
        var studentId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(studentId)) return Forbid();

        var me = await _userManager.GetUserAsync(User);
        var myFacultyId = me?.FacultyId ?? 0;

        var tutorUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == vm.TutorUserId);
        if (tutorUser == null) return NotFound();

        ViewBag.TutorName = tutorUser.Name;

        ViewBag.Subjects = myFacultyId != 0
            ? await _context.Subjects.Where(s => s.FacultyId == myFacultyId).OrderBy(s => s.Name).ToListAsync()
            : await _context.Subjects.OrderBy(s => s.Name).ToListAsync();

        if (!ModelState.IsValid) return View(vm);

        if (myFacultyId != 0)
        {
            var okSubject = await _context.Subjects.AnyAsync(s => s.Id == vm.SubjectId && s.FacultyId == myFacultyId);
            if (!okSubject)
            {
                ModelState.AddModelError(nameof(vm.SubjectId), "Invalid subject for your faculty.");
                return View(vm);
            }

            var tutorFacultyId = await _context.Tutors.Where(t => t.Id == vm.TutorUserId).Select(t => (int?)t.FacultyId).FirstOrDefaultAsync();
            if (tutorFacultyId.HasValue && tutorFacultyId.Value != myFacultyId)
                return Forbid();
        }

        var req = new TutorRequest
        {
            StudentUserId = studentId,
            TutorUserId = vm.TutorUserId,
            SubjectId = vm.SubjectId,
            Message = vm.Message,
            PreferredTime = vm.PreferredTime,
            IsOnline = vm.IsOnline,
            Status = TutorRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.TutorRequests.Add(req);
        await _context.SaveChangesAsync();

        _context.TutorRequestMessages.Add(new TutorRequestMessage
        {
            TutorRequestId = req.Id,
            SenderUserId = studentId,
            Body = vm.Message,
            SentAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(tutorUser.Email))
        {
            var subjectName = await _context.Subjects
                .Where(s => s.Id == vm.SubjectId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync() ?? "Subject";

            var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == studentId);
            var studentName = student?.Name ?? "Student";
            var studentEmail = student?.Email ?? "";

            var html = $@"
                <h3>New tutoring request</h3>
                <p><b>{studentName}</b> ({studentEmail}) requested a session for <b>{subjectName}</b>.</p>
                <p><b>Mode:</b> {(vm.IsOnline ? "Online" : "On campus")}</p>
                <p><b>Preferred time:</b> {System.Net.WebUtility.HtmlEncode(vm.PreferredTime ?? "-")}</p>
                <p><b>Message:</b><br/>{System.Net.WebUtility.HtmlEncode(vm.Message).Replace("\n", "<br/>")}</p>
                <p>Open StudyBuddy → Tutors → Requests/My requests.</p>
            ";

            await _email.SendAsync(tutorUser.Email, "StudyBuddy: New tutoring request", html);
        }

        TempData["ok"] = "Request sent! You can continue the conversation here.";
        return RedirectToAction(nameof(RequestDetails), new { id = req.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Requests()
    {
        var tutorId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(tutorId)) return Forbid();

        var list = await _context.TutorRequests
            .Include(r => r.Subject)
            .Include(r => r.StudentUser)
            .Where(r => r.TutorUserId == tutorId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new TutorRequestListItemVM
            {
                Id = r.Id,
                StudentName = r.StudentUser != null ? r.StudentUser.Name : "",
                StudentEmail = r.StudentUser != null ? (r.StudentUser.Email ?? "") : "",
                SubjectName = r.Subject != null ? r.Subject.Name : "",
                Message = r.Message,
                PreferredTime = r.PreferredTime,
                IsOnline = r.IsOnline,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        ViewBag.IsStudentView = false;
        return View(list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decide(int id, bool accept, string? responseMessage)
    {
        var tutorId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(tutorId)) return Forbid();

        var req = await _context.TutorRequests
            .Include(r => r.Subject)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (req == null) return NotFound();
        if (req.TutorUserId != tutorId) return Forbid();

        req.Status = accept ? TutorRequestStatus.Accepted : TutorRequestStatus.Declined;
        req.RespondedAt = DateTime.UtcNow;
        req.TutorResponseMessage = responseMessage;

        await _context.SaveChangesAsync();

        var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == req.StudentUserId);
        var tutor = await _context.Users.FirstOrDefaultAsync(u => u.Id == tutorId);

        if (!string.IsNullOrWhiteSpace(student?.Email))
        {
            var subj = req.Subject?.Name ?? "Subject";
            var tutorName = tutor?.Name ?? "Tutor";
            var statusText = accept ? "accepted" : "declined";

            var html = $@"
                <h3>Your tutoring request was {statusText}</h3>
                <p><b>Tutor:</b> {tutorName}</p>
                <p><b>Subject:</b> {subj}</p>
                <p><b>Status:</b> {statusText}</p>
                <p><b>Message from tutor:</b><br/>{System.Net.WebUtility.HtmlEncode(responseMessage ?? "-").Replace("\n", "<br/>")}</p>
            ";

            await _email.SendAsync(student.Email, $"StudyBuddy: Tutor {statusText} your request", html);
        }

        if (!string.IsNullOrWhiteSpace(responseMessage))
        {
            _context.TutorRequestMessages.Add(new TutorRequestMessage
            {
                TutorRequestId = req.Id,
                SenderUserId = tutorId,
                Body = responseMessage.Trim(),
                SentAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        TempData["ok"] = accept ? "Request accepted." : "Request declined.";
        return RedirectToAction(nameof(Requests));
    }

    [HttpGet]
    public async Task<IActionResult> RequestDetails(int id)
    {
        var meId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(meId)) return Forbid();

        var req = await _context.TutorRequests
            .Include(r => r.Subject)
            .Include(r => r.StudentUser)
            .Include(r => r.TutorUser)
            .Include(r => r.Messages)
                .ThenInclude(m => m.SenderUser)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (req == null) return NotFound();

        if (req.StudentUserId != meId && req.TutorUserId != meId)
            return Forbid();

        req.Messages = req.Messages.OrderBy(m => m.SentAt).ToList();
        return View(req);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendRequestMessage(int id, string body)
    {
        var meId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(meId)) return Forbid();

        var req = await _context.TutorRequests
            .Include(r => r.StudentUser)
            .Include(r => r.TutorUser)
            .Include(r => r.Subject)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (req == null) return NotFound();
        if (req.StudentUserId != meId && req.TutorUserId != meId) return Forbid();

        body = (body ?? "").Trim();
        if (body.Length == 0)
        {
            TempData["err"] = "Message cannot be empty.";
            return RedirectToAction(nameof(RequestDetails), new { id });
        }

        if (body.Length > 2000) body = body[..2000];

        _context.TutorRequestMessages.Add(new TutorRequestMessage
        {
            TutorRequestId = id,
            SenderUserId = meId,
            Body = body,
            SentAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var isTutorSending = meId == req.TutorUserId;
        var toEmail = isTutorSending ? req.StudentUser?.Email : req.TutorUser?.Email;

        if (!string.IsNullOrWhiteSpace(toEmail))
        {
            var subjName = req.Subject?.Name ?? "Subject";
            var fromName = isTutorSending ? (req.TutorUser?.Name ?? "Tutor") : (req.StudentUser?.Name ?? "Student");

            var html = $@"
                <h3>New message in tutoring request</h3>
                <p><b>Subject:</b> {subjName}</p>
                <p><b>From:</b> {System.Net.WebUtility.HtmlEncode(fromName)}</p>
                <p><b>Message:</b><br/>{System.Net.WebUtility.HtmlEncode(body).Replace("\n", "<br/>")}</p>
            ";

            await _email.SendAsync(toEmail, "StudyBuddy: New tutoring message", html);
        }

        TempData["ok"] = "Message sent.";
        return RedirectToAction(nameof(RequestDetails), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> MyRequests()
    {
        var studentId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(studentId)) return Forbid();

        var list = await _context.TutorRequests
            .Include(r => r.Subject)
            .Where(r => r.StudentUserId == studentId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new TutorRequestListItemVM
            {
                Id = r.Id,
                StudentName = "",
                StudentEmail = "",
                SubjectName = r.Subject != null ? r.Subject.Name : "",
                Message = r.Message,
                PreferredTime = r.PreferredTime,
                IsOnline = r.IsOnline,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        ViewBag.IsStudentView = true;
        return View("Requests", list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRequest(int id)
    {
        var meId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(meId)) return Forbid();

        var req = await _context.TutorRequests.FirstOrDefaultAsync(r => r.Id == id);
        if (req == null) return NotFound();

        if (req.StudentUserId != meId && req.TutorUserId != meId)
            return Forbid();

        _context.TutorRequests.Remove(req);
        await _context.SaveChangesAsync();

        TempData["ok"] = "Request deleted.";

        var isTutor = req.TutorUserId == meId;
        return RedirectToAction(isTutor ? nameof(Requests) : nameof(MyRequests));
    }
}
