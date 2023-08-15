using System.ComponentModel.DataAnnotations;

namespace NewsAPIProject.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
