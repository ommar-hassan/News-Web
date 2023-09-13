using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Models
{
    public class Author : IdentityUser
    {
        [StringLength(20, MinimumLength = 3)]
        public string FirstName { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string LastName { get; set; }
    }
}
