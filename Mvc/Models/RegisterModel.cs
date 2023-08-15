using System.ComponentModel.DataAnnotations;

namespace Mvc.Models
{
    public class RegisterModel
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }
        public string username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
