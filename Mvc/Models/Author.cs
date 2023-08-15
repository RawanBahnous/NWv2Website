using System.ComponentModel.DataAnnotations;

namespace Mvc.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
