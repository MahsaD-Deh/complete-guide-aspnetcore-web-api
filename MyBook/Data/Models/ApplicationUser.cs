using Microsoft.AspNetCore.Identity;

namespace MyBook.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Custom { get; set; }
    }
}
