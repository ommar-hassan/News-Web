using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Dto
{
    public class CreateNewsDto
    {
        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(1500)]
        public string Description { get; set; }
        public IFormFile Image { get; set; }

        [Display(Name = "Author")]
        [Required(ErrorMessage = "Author is required.")]
        public string AuthorId { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}
