using Microsoft.AspNetCore.Identity;

namespace AiCademy.Web.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
