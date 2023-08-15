using System.ComponentModel.DataAnnotations;

namespace Mvc.Models
{
    public class RoleModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}
