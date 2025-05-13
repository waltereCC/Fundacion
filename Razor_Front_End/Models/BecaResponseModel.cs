namespace Razor_Front_End.Models
{
    public class BecaResponseModel
    {
        public int Id { get; set; }
        public string NombrePadreMadre { get; set; }
        public string NombreEstudiante { get; set; }
        public string CedulaPadreMadre { get; set; }
        public int EdadEstudiante { get; set; }
        public string Direccion { get; set; }
        public string Estado { get; set; }
        public string ArchivoNotasUrl { get; set; }
        public string ArchivoCartaFirmadaUrl { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioEmail { get; set; }

        public string NuevoEstado { get; set; }
    }
}
