using System.ComponentModel.DataAnnotations;

namespace NewsApp.Models
{
    public class CreateNewsViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        [MinLength(5, ErrorMessage = "Title cannot be shorter than 5 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(1500, ErrorMessage = "Description cannot be longer than 1500 characters")]
        [MinLength(10, ErrorMessage = "Description cannot be shorter than 10 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Image")]
        public IFormFile Image { get; set; }

        [Display(Name = "Author")]
        [Required(ErrorMessage = "Author is required")]
        public string AuthorId { get; set; }

        [Required(ErrorMessage = "PublicationDate is required")]
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }
    }
}
