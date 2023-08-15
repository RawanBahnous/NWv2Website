using System.ComponentModel.DataAnnotations;

namespace Mvc.Models
{
    public class LoginModel
    {
       
        [Required(ErrorMessage = "Username is required.")]
        public string Email { get; set; }
        //[Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",
        ErrorMessage = "The password must be at least 8 characters long and contain at least one letter, one digit, and one special character.")]


        public string Password { get; set; }
    }
}
