using System.ComponentModel.DataAnnotations;

namespace NewsApp.Models
{
    public class UpdateAuthorViewModel
    {
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First name should not be more that 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last name should not be more that 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username should be between 3 and 20 characters.")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(128, ErrorMessage = "Email should not be more that 128 characters.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

    }
}
