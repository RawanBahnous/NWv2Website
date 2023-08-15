using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Mvc.Models
{
    public class CreateNewsModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [MaxLength(200)]
        public string Content { get; set; }

        [Required]
        [Display(Name = "Publication Date")]
        public DateTime PublicationDate { get; set; }

        [Required]
        [Display(Name = "Author ID")]
        public int AuthorID { get; set; }

        [Display(Name = "Image")]
        public string ImageFile { get; set; }

        public List<SelectListItem> Authors { get; set; }
    }
}
