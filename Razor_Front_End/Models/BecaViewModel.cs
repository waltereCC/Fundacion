namespace Razor_Front_End.Models
{
    public class BecaViewModel
    {
        public int Id { get; set; }
        public string NombrePadreMadre { get; set; }
        public string NombreEstudiante { get; set; }
        public string CedulaPadreMadre { get; set; }
        public int EdadEstudiante { get; set; }
        public string Direccion { get; set; }
        public IFormFile ArchivoNotas { get; set; }
        public IFormFile ArchivoCartaFirmada { get; set; }

        public string Estado { get; set; } = "Enviada para Revisión";
    }
}
