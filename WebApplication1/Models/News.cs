using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsAPIProject.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [Required]
        [MaxLength(200)]
        public string Content { get; set; }
        [Required(ErrorMessage = "Publication date is required.")]
        [Display(Name = "Publication Date")]
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Image { get; set; }
        [ForeignKey(nameof(Author))]
        public int AuthorID { get; set; }
        public Author? AuthorName { get; set; }

    }
}
