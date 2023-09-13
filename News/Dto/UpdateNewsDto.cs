using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Dto
{
    public class UpdateNewsDto
    {
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        public string AuthorId { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}
