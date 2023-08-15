using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NewsAPIProject.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
    }
}
