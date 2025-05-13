using System.ComponentModel.DataAnnotations;

namespace Razor_Front_End.Models
{
    public class UserUpdateViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Cedula { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }
    }
}
