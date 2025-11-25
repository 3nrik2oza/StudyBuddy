using Microsoft.AspNetCore.Identity;

namespace web.Models;

public class ApplicationUser: IdentityUser
{
    
        public string Name { get; set; }  = string.Empty;          
        public string? Description { get; set; }
        public int FacultyId { get; set; }
        public bool IsTutor { get; set; }
}