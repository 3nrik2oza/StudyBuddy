#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using web.Data;
using web.Models.Entities;
using web.Models;

namespace web.Areas.Identity.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly StudyBuddyDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProfileModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            StudyBuddyDbContext context,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _env = env;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public SelectList FacultyOptions { get; set; }
        public SelectList SubjectOptions { get; set; }

        public List<StudyPost> UserStudyPosts { get; set; } = new();
        public List<Material> UserMaterials { get; set; } = new();

        public int StudyPostsCount { get; set; }
        public int MaterialsCount { get; set; }

        // Upload field
        [BindProperty]
        public IFormFile AvatarFile { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int FacultyId { get; set; }
            public string FacultyName { get; set; }
            public bool IsTutor { get; set; }

            public List<int> SubjectIds { get; set; } = new List<int>();

            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }

            // NEW
            public string ProfilePicturePath { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            FacultyOptions = new SelectList(await _context.Faculties.ToListAsync(), "Id", "Name");
            SubjectOptions = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name");

            var faculty = await _context.Faculties.FindAsync(user.FacultyId);

            var selectedSubjects = await _context.TutorSubjects
                .Where(ts => ts.UserId == user.Id)
                .Select(ts => ts.SubjectId)
                .ToListAsync();

            UserStudyPosts = await _context.StudyPosts
                .Include(s => s.Subject)
                .Include(s => s.Faculty)
                .Where(s => s.AuthorUserId == user.Id)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            UserMaterials = await _context.Materials
                .Include(m => m.Subject)
                .Include(m => m.Faculty)
                .Where(m => m.AuthorUserId == user.Id)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            StudyPostsCount = UserStudyPosts.Count;
            MaterialsCount = UserMaterials.Count;

            Input = new InputModel
            {
                Email = user.Email,
                Name = user.Name,
                Description = user.Description,
                FacultyId = user.FacultyId,
                FacultyName = faculty?.Name ?? "",
                IsTutor = user.IsTutor,
                SubjectIds = selectedSubjects,
                ProfilePicturePath = user.ProfilePicturePath
            };

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            FacultyOptions = new SelectList(await _context.Faculties.ToListAsync(), "Id", "Name");
            SubjectOptions = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name");

            if (!ModelState.IsValid)
                return Page();

            user.Name = Input.Name;
            user.Description = Input.Description;
            user.FacultyId = Input.FacultyId;
            user.IsTutor = Input.IsTutor;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            if (user.IsTutor)
            {
                var tutor = await _context.Tutors.FindAsync(user.Id);
                if (tutor == null)
                {
                    tutor = new Tutor
                    {
                        Id = user.Id,
                        Name = user.Name,
                        FacultyId = user.FacultyId,
                        HelpPoints = 0
                    };
                    _context.Tutors.Add(tutor);
                }
                else
                {
                    tutor.Name = user.Name;
                    tutor.FacultyId = user.FacultyId;
                }

                var existing = _context.TutorSubjects.Where(ts => ts.UserId == user.Id);
                _context.TutorSubjects.RemoveRange(existing);

                if (Input.SubjectIds != null)
                {
                    foreach (var subjectId in Input.SubjectIds)
                    {
                        _context.TutorSubjects.Add(new TutorSubject
                        {
                            UserId = user.Id,
                            SubjectId = subjectId
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                var tutor = await _context.Tutors.FindAsync(user.Id);
                if (tutor != null)
                {
                    _context.Tutors.Remove(tutor);

                    var links = _context.TutorSubjects.Where(ts => ts.UserId == user.Id);
                    _context.TutorSubjects.RemoveRange(links);

                    await _context.SaveChangesAsync();
                }
            }

            TempData["StatusMessage"] = "Profile updated successfully!";
            return RedirectToPage();
        }

        // NEW: upload / change profile picture
        public async Task<IActionResult> OnPostUploadAvatarAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (AvatarFile == null || AvatarFile.Length == 0)
            {
                TempData["StatusMessage"] = "Please select an image file.";
                return RedirectToPage();
            }

            const long maxSize = 3 * 1024 * 1024; // 3MB
            if (AvatarFile.Length > maxSize)
            {
                TempData["StatusMessage"] = "Image is too large (max 3MB).";
                return RedirectToPage();
            }

            var ext = Path.GetExtension(AvatarFile.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext))
            {
                TempData["StatusMessage"] = "Only JPG, PNG or WEBP images are allowed.";
                return RedirectToPage();
            }

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "avatars");
            Directory.CreateDirectory(uploadsDir);

            var safeUserId = user.Id.Replace(":", "_");
            var random = Convert.ToHexString(RandomNumberGenerator.GetBytes(8)).ToLowerInvariant();
            var fileName = $"{safeUserId}_{random}{ext}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            await using (var stream = System.IO.File.Create(fullPath))
            {
                await AvatarFile.CopyToAsync(stream);
            }

            // delete old file (optional)
            if (!string.IsNullOrWhiteSpace(user.ProfilePicturePath))
            {
                var oldRel = user.ProfilePicturePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                var oldFull = Path.Combine(_env.WebRootPath, oldRel);
                if (System.IO.File.Exists(oldFull))
                {
                    try { System.IO.File.Delete(oldFull); } catch { }
                }
            }

            user.ProfilePicturePath = $"/uploads/avatars/{fileName}";
            await _userManager.UpdateAsync(user);

            TempData["StatusMessage"] = "Profile picture updated!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
                return Page();

            if (Input.NewPassword != Input.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "New password and confirmation do not match.");
                return Page();
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                Input.CurrentPassword,
                Input.NewPassword
            );

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "Password changed successfully!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostDeleteStudyPostAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var post = await _context.StudyPosts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound();
            if (post.AuthorUserId != user.Id) return Forbid();

            _context.StudyPosts.Remove(post);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Study post deleted.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteMaterialAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var material = await _context.Materials.FirstOrDefaultAsync(m => m.Id == id);
            if (material == null) return NotFound();
            if (material.AuthorUserId != user.Id) return Forbid();

            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Material deleted.";
            return RedirectToPage();
        }
    }
}
