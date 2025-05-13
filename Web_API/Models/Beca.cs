using System.ComponentModel.DataAnnotations;

namespace Web_API.Models
{
    public class Beca
    {
        public int Id { get; set; }

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

        [Required]
        public string ArchivoNotas { get; set; } 

        [Required]
        public string ArchivoCartaFirmada { get; set; } 

        [Required]
        public string UsuarioId { get; set; } 

        public DateTime FechaSolicitud { get; set; }

        public string Estado { get; set; } = "Enviada para Revisión";
    }
}
