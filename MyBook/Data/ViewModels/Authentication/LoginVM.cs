using System.ComponentModel.DataAnnotations;

namespace MyBook.Data.ViewModels.Authentication
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email is required!!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Psssword is required!!")]
        public string Password { get; set; }


    }
}
