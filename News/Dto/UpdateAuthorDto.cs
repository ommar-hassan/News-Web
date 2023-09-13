using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Dto
{
    public class UpdateAuthorDto
    {
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(20, MinimumLength = 3)]
        public string UserName { get; set; }
        [StringLength(128)]
        public string Email { get; set; }
    }
}
