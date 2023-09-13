using NewsAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Dto
{
    public class NewsDto
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; }
        [MaxLength(3000)]
        public string ImageUrl { get; set; }
        public AuthorDto Author { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
