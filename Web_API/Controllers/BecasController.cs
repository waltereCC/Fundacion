using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_API.Data;
using Web_API.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Web_API.Interfaces;

namespace Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BecasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public BecasController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("postBeca")]
        public async Task<IActionResult> PostBeca([FromForm] BecaFormModel model, IFormFile archivoNotas, IFormFile archivoCartaFirmada)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var userIdInt = int.Parse(userId);
            var user = await _context.Users.FindAsync(userIdInt);
            if (user == null)
            {
                return NotFound();
            }

            var beca = new Beca
            {
                NombrePadreMadre = model.NombrePadreMadre,
                NombreEstudiante = model.NombreEstudiante,
                CedulaPadreMadre = model.CedulaPadreMadre,
                EdadEstudiante = model.EdadEstudiante,
                Direccion = model.Direccion,
                UsuarioId = userId,
                FechaSolicitud = DateTime.UtcNow,
                Estado = "Enviada para Revisión"
            };

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Manejo de archivos
            if (archivoNotas != null && archivoNotas.Length > 0)
            {

                var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                var fileExtension = Path.GetExtension(archivoNotas.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError(string.Empty, "Archivo no permitido.");
                    return BadRequest(ModelState);
                }

                var archivoNotasPath = Path.Combine("Uploads", archivoNotas.FileName);
                using (var stream = new FileStream(archivoNotasPath, FileMode.Create))
                {
                    await archivoNotas.CopyToAsync(stream);
                }
                beca.ArchivoNotas = archivoNotasPath;
            }

            if (archivoCartaFirmada != null && archivoCartaFirmada.Length > 0)
            {
                var archivoCartaPath = Path.Combine("Uploads", Path.GetRandomFileName() + Path.GetExtension(archivoCartaFirmada.FileName));
                using (var stream = new FileStream(archivoCartaPath, FileMode.Create))
                {
                    await archivoCartaFirmada.CopyToAsync(stream);
                }
                beca.ArchivoCartaFirmada = archivoCartaPath;
            }

            _context.Becas.Add(beca);
            await _context.SaveChangesAsync();

            var adminEmail = "correo@correo.com";
            var subject = "Nueva Solicitud de Beca";
            var message = $"Se ha solicitado una nueva beca para {model.NombreEstudiante}. Por favor, revisa la solicitud en el sistema.";

            await _emailService.SendEmailAsync(adminEmail, subject, message);

            return CreatedAtAction(nameof(GetBeca), new { id = beca.Id }, beca);
        }

        [HttpGet("getAllBecas")]
        public async Task<IActionResult> GetAllBecas()
        {
            var baseUrl = "http://localhost:5116/api/Becas/uploads/";

            var becasList = await _context.Becas.ToListAsync();
            var usersList = await _context.Users.ToListAsync();

            var becas = from b in becasList
                        join u in usersList on int.Parse(b.UsuarioId) equals u.Id
                        select new BecaResponseModel
                        {
                            Id = b.Id,
                            NombrePadreMadre = b.NombrePadreMadre,
                            NombreEstudiante = b.NombreEstudiante,
                            CedulaPadreMadre = b.CedulaPadreMadre,
                            EdadEstudiante = b.EdadEstudiante,
                            Direccion = b.Direccion,
                            Estado = b.Estado,
                            ArchivoNotasUrl = Path.Combine(baseUrl, Path.GetFileName(b.ArchivoNotas)).Replace("\\", "/"),
                            ArchivoCartaFirmadaUrl = Path.Combine(baseUrl, Path.GetFileName(b.ArchivoCartaFirmada)).Replace("\\", "/"),
                            FechaSolicitud = b.FechaSolicitud,
                            UsuarioNombre = u.Nombre,
                            UsuarioEmail = u.Email
                        };

            return Ok(becas.ToList());
        }

        [HttpPost("updateBeca/{id}")]
        public async Task<IActionResult> UpdateBeca(int id, [FromForm] BecaFormModel model, IFormFile archivoNotas, IFormFile archivoCartaFirmada)
        {
            // Verifica si el modelo es válido
            if (!ModelState.IsValid)
            {
                return BadRequest("Modelo inválido.");
            }

            // Busca la beca a actualizar
            var beca = await _context.Becas.FindAsync(id);
            if (beca == null)
            {
                return NotFound();
            }

            // Verifica que la beca esté en el estado correcto para actualizar
            if (beca.Estado != "Enviada para Revisión")
            {
                return BadRequest("La beca no está en estado 'Enviada para Revisión'.");
            }

            // Actualizar la información de la beca
            beca.NombrePadreMadre = model.NombrePadreMadre;
            beca.NombreEstudiante = model.NombreEstudiante;
            beca.CedulaPadreMadre = model.CedulaPadreMadre;
            beca.EdadEstudiante = model.EdadEstudiante;
            beca.Direccion = model.Direccion;

            // Manejo de archivos (similar al método PostBeca)
            if (archivoNotas != null && archivoNotas.Length > 0)
            {
                var archivoNotasPath = Path.Combine("Uploads", archivoNotas.FileName);
                using (var stream = new FileStream(archivoNotasPath, FileMode.Create))
                {
                    await archivoNotas.CopyToAsync(stream);
                }
                beca.ArchivoNotas = archivoNotasPath;
            }

            if (archivoCartaFirmada != null && archivoCartaFirmada.Length > 0)
            {
                var archivoCartaPath = Path.Combine("Uploads", archivoCartaFirmada.FileName);
                using (var stream = new FileStream(archivoCartaPath, FileMode.Create))
                {
                    await archivoCartaFirmada.CopyToAsync(stream);
                }
                beca.ArchivoCartaFirmada = archivoCartaPath;
            }

            _context.Becas.Update(beca);
            await _context.SaveChangesAsync();

            return NoContent();
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetBeca(int id)
        {
            var beca = await _context.Becas.FindAsync(id);

            if (beca == null)
            {
                return NotFound();
            }

            return Ok(beca);
        }

        [HttpGet("getBecaById/{id}")]
        public async Task<IActionResult> GetBecaById(int id)
        {
            var beca = await _context.Becas.FindAsync(id);
            if (beca == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var baseUrl = "http://localhost:5116/api/Becas/uploads/";
            var becaResponse = new BecaResponseModel
            {
                NombrePadreMadre = beca.NombrePadreMadre,
                NombreEstudiante = beca.NombreEstudiante,
                CedulaPadreMadre = beca.CedulaPadreMadre,
                EdadEstudiante = beca.EdadEstudiante,
                Direccion = beca.Direccion,
                Estado = beca.Estado,
                ArchivoNotasUrl = Path.Combine(baseUrl, Path.GetFileName(beca.ArchivoNotas)).Replace("\\", "/"),
                ArchivoCartaFirmadaUrl = Path.Combine(baseUrl, Path.GetFileName(beca.ArchivoCartaFirmada)).Replace("\\", "/"),
                FechaSolicitud = beca.FechaSolicitud
            };

            return Ok(becaResponse);
        }

        [HttpGet("getBecasByUser")]
        public async Task<IActionResult> GetBecasByUser()
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var baseUrl = "http://localhost:5116/api/Becas/uploads/";

            var becas = await _context.Becas
                                      .Where(b => b.UsuarioId == userId)
                                      .Select(b => new BecaResponseModel
                                      {
                                          Id = b.Id,
                                          NombrePadreMadre = b.NombrePadreMadre,
                                          NombreEstudiante = b.NombreEstudiante,
                                          CedulaPadreMadre = b.CedulaPadreMadre,
                                          EdadEstudiante = b.EdadEstudiante,
                                          Direccion = b.Direccion,
                                          Estado = b.Estado,
                                          ArchivoNotasUrl = Path.Combine(baseUrl, Path.GetFileName(b.ArchivoNotas)).Replace("\\", "/"),
                                          ArchivoCartaFirmadaUrl = Path.Combine(baseUrl, Path.GetFileName(b.ArchivoCartaFirmada)).Replace("\\", "/"),
                                          FechaSolicitud = b.FechaSolicitud
                                      })
                                      .ToListAsync();

            return Ok(becas);
        }


        [HttpGet("uploads/{fileName}")]
        public IActionResult GetFile(string fileName)
        {

            var filePath = Path.Combine("Uploads", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }

        [HttpPut("updateEstado/{id}")]
        public async Task<IActionResult> UpdateEstado(int id, [FromBody] UpdateEstadoBecaRequest request)
        {
            if (request == null)
            {
                return BadRequest("Solicitud inválida.");
            }

            var beca = await _context.Becas.FindAsync(id);
            if (beca == null)
            {
                return NotFound();
            }

            // Validar el estado si es necesario
            var validStates = new[] { "Enviada para Revisión", "En Revisión", "Aceptada", "Rechazada" };
            if (!validStates.Contains(request.Estado))
            {
                return BadRequest("Estado no válido.");
            }

            beca.Estado = request.Estado;
            _context.Becas.Update(beca);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("deleteBeca/{id}")]
        public async Task<IActionResult> DeleteBeca(int id)
        {
            var beca = await _context.Becas.FindAsync(id);
            if (beca == null)
            {
                return NotFound();
            }

            if (beca.Estado != "Enviada para Revisión")
            {
                return BadRequest("La beca no está en estado 'Enviada para Revisión'.");
            }

            _context.Becas.Remove(beca);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Obtiene datos del usuario para completarlos automaticamente en el formulario de nueva beca
        [HttpGet("obtenerDatos")]
        public async Task<IActionResult> ObtenerDatosUsuario()
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var userIdInt = int.Parse(userId);
            var user = await _context.Users.FindAsync(userIdInt);
            if (user == null)
            {
                return NotFound();
            }

            var usuarioDatos = new
            {
                nombreCompleto = user.Nombre,
                cedula = user.Cedula
            };

            return Ok(usuarioDatos);
        }

        [HttpGet("generateBecasReport")]
        public async Task<IActionResult> GenerateBecasReport()
        {

            var becas = await (from b in _context.Becas
                               join u in _context.Users on b.UsuarioId equals u.Id.ToString()
                               select new
                               {
                                   b.NombrePadreMadre,
                                   b.NombreEstudiante,
                                   b.CedulaPadreMadre,
                                   b.Estado,
                                   b.FechaSolicitud,
                                   UsuarioNombre = u.Nombre,
                                   UsuarioEmail = u.Email
                               }).ToListAsync();

            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                var table = new PdfPTable(6);
                table.AddCell("Nombre del Padre/Madre");
                table.AddCell("Nombre del Estudiante");
                table.AddCell("Cédula del Padre/Madre");
                table.AddCell("Estado");
                table.AddCell("Fecha de Solicitud");
                table.AddCell("Nombre del Usuario");

                foreach (var beca in becas)
                {
                    table.AddCell(beca.NombrePadreMadre ?? "");
                    table.AddCell(beca.NombreEstudiante ?? "");
                    table.AddCell(beca.CedulaPadreMadre ?? "");
                    table.AddCell(beca.Estado ?? "");
                    table.AddCell(beca.FechaSolicitud.ToString("dd/MM/yyyy"));
                    table.AddCell(beca.UsuarioNombre ?? "");
                }

                document.Add(table);
                document.Close();

                var fileName = "BecasReport.pdf";
                return File(ms.ToArray(), "application/pdf", fileName);
            }
        }
    }
}
