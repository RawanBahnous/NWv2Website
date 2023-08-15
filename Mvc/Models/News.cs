using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mvc.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [MaxLength(200)]
        public string Content { get; set; }
        //[Required]
        public DateTime PublicationDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? Image { get; set; }
        [ForeignKey(nameof(Author))]
        public int AuthorID { get; set; }
        public Author? AuthorName { get; set; }

    }
}
