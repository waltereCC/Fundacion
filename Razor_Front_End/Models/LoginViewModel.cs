using System.ComponentModel.DataAnnotations;

namespace Razor_Front_End.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

}
