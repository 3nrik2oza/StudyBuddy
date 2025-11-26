#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using web.Data;
using web.Models;
using web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly StudyBuddyDbContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            StudyBuddyDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public SelectList FacultyOptions { get; set; }
        public SelectList SubjectOptions { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string Name { get; set; }

            [Display(Name = "Description")]
            public string Description { get; set; }

            [Required]
            [Display(Name = "Faculty")]
            public int FacultyId { get; set; }

            [Display(Name = "Are you a Tutor?")]
            public bool IsTutor { get; set; }

            [Display(Name = "Subjects you teach")]
            public List<int> SubjectIds { get; set; } = new List<int>();
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Load faculties + subjects for dropdowns
            FacultyOptions = new SelectList(await _context.Faculties.ToListAsync(), "Id", "Name");
            SubjectOptions = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Reload dropdowns when form returns with error
            FacultyOptions = new SelectList(await _context.Faculties.ToListAsync(), "Id", "Name");
            SubjectOptions = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name");

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                // Assign main user properties
                user.Name = Input.Name;
                user.Description = Input.Description;
                user.FacultyId = Input.FacultyId;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account.");

                    // ⭐ CREATE TUTOR ENTITY IF USER SELECTED "IS TUTOR"
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

                        // Add TutorSubject relationships
                        if (Input.SubjectIds != null && Input.SubjectIds.Count > 0)
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

                    // Email confirmation logic
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        null,
                        new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                // Display errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If invalid, reload page
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. Make sure it has a parameterless constructor.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Identity user store does not support emails.");
            }

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
