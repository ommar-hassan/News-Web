using System.ComponentModel.DataAnnotations;

namespace NewsApp.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Enter a valid Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter a valid Password.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
