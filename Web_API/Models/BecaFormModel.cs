using System.ComponentModel.DataAnnotations;

namespace Web_API.Models
{
    public class BecaFormModel
    {
        [Required]
        public string NombrePadreMadre { get; set; }

        [Required]
        public string NombreEstudiante { get; set; }

        [Required]
        public string CedulaPadreMadre { get; set; }

        [Required]
        public int EdadEstudiante { get; set; }

        [Required]
        public string Direccion { get; set; }
    }
}
