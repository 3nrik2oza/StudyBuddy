#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web.Models.Entities;
using web.Models;
using web.Data;

namespace web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly StudyBuddyDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            StudyBuddyDbContext context,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public SelectList FacultyOptions { get; set; }
        public List<SelectListItem> SubjectOptions { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

            public string Description { get; set; }

            [Required]
            public int FacultyId { get; set; }

            public bool IsTutor { get; set; }

            public List<int> SubjectIds { get; set; } = new();
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            FacultyOptions = new SelectList(await _context.Faculties.ToListAsync(), "Id", "Name");
            SubjectOptions = new List<SelectListItem>();

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            FacultyOptions = new SelectList(await _context.Faculties.ToListAsync(), "Id", "Name");
            SubjectOptions = new List<SelectListItem>();

            if (!ModelState.IsValid)
                return Page();
                

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                Name = Input.Name,
                Description = Input.Description,
                FacultyId = Input.FacultyId,
                IsTutor = Input.IsTutor
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            _logger.LogInformation("User created a new account.");


            if (Input.IsTutor)
            {
                var tutor = new Tutor
                {
                    Id = user.Id,
                    Name = user.Name,
                    FacultyId = user.FacultyId,
                    HelpPoints = 0
                };
                _context.Tutors.Add(tutor);

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


            await _signInManager.SignInAsync(user, isPersistent: false);

            return LocalRedirect(returnUrl);
        }
    }
}
