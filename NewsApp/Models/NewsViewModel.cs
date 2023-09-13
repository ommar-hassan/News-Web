using System.ComponentModel.DataAnnotations;

namespace NewsApp.Models
{
    public class NewsViewModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; }
        [Display(Name = "Image")]
        [MaxLength(3000)]
        public string ImageUrl { get; set; }
        public AuthorViewModel Author { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
