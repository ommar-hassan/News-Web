using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Models
{
    public class News
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; }
        [MaxLength(3000)]
        public string Image { get; set; }
        public string AuthorId { get; set; }
        public Author Author { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now.ToLocalTime();
    }
}
