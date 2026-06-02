using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PasswordRequired { get; set; }

        public bool RememberMe { get; set; } // للـ checkbox
    }
}
