using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using Razor_Front_End.Models;
using Razor_Front_End.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using static Razor_Front_End.Services.ApiService;

namespace Razor_Front_End.Controllers
{
    public class BecasController : Controller
    {
        private readonly HttpClient _client;
        private readonly ApiService _apiService;

        public BecasController(IHttpClientFactory httpClientFactory, ApiService apiService)
        {
            _client = httpClientFactory.CreateClient("ApiClient");
            _apiService = apiService;
        }


        [HttpGet]
        public async Task<IActionResult> SolicitarNuevaBeca()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
                ViewBag.UserRole = roleClaim?.Value ?? "guest";
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { message = "Error al procesar el token de autenticación." });
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Becas/obtenerDatos");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Error", "Home"); 
            }

            UserBecasModel userData;
            try
            {
                userData = await response.Content.ReadFromJsonAsync<UserBecasModel>();
                if (userData == null)
                {
                    throw new Exception("No se pudieron deserializar los datos del usuario.");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { message = ex.Message });
            }

            var model = new BecaViewModel
            {
                NombrePadreMadre = userData.NombreCompleto,
                CedulaPadreMadre = userData.Cedula,
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> SolicitarNuevaBeca(BecaViewModel model)
        {

            var token = HttpContext.Session.GetString("JwtToken");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(model.NombrePadreMadre), "NombrePadreMadre");
                content.Add(new StringContent(model.NombreEstudiante), "NombreEstudiante");
                content.Add(new StringContent(model.CedulaPadreMadre), "CedulaPadreMadre");
                content.Add(new StringContent(model.EdadEstudiante.ToString()), "EdadEstudiante");
                content.Add(new StringContent(model.Direccion), "Direccion");
                content.Add(new StringContent(model.Estado), "Estado");

                if (model.ArchivoNotas != null)
                {
                    var archivoNotasContent = new StreamContent(model.ArchivoNotas.OpenReadStream());
                    archivoNotasContent.Headers.ContentType = new MediaTypeHeaderValue(model.ArchivoNotas.ContentType);
                    content.Add(archivoNotasContent, "archivoNotas", model.ArchivoNotas.FileName);
                }

                if (model.ArchivoCartaFirmada != null)
                {
                    var archivoCartaContent = new StreamContent(model.ArchivoCartaFirmada.OpenReadStream());
                    archivoCartaContent.Headers.ContentType = new MediaTypeHeaderValue(model.ArchivoCartaFirmada.ContentType);
                    content.Add(archivoCartaContent, "archivoCartaFirmada", model.ArchivoCartaFirmada.FileName);
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _client.PostAsync("/api/Becas/postBeca", content);
                

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("MisBecas", "Becas");
                }

                ModelState.AddModelError(string.Empty, "Error al enviar la solicitud.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditarBeca(int id)
        {

            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    if (jwtToken == null)
                    {
                        ViewBag.UserRole = "guest";
                    }
                    else
                    {
                        var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
                        ViewBag.UserRole = roleClaim?.Value ?? "guest";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.UserRole = "guest";
                }
            }

            
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Becas/getBecaById/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Error", "Home");
            }

            var beca = await response.Content.ReadFromJsonAsync<BecaViewModel>();
            return View(beca);
        }

        [HttpPost]
        public async Task<IActionResult> EditarBeca(int id, BecaViewModel model, IFormFile archivoNotas, IFormFile archivoCartaFirmada)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(model.NombrePadreMadre), "NombrePadreMadre");
                content.Add(new StringContent(model.NombreEstudiante), "NombreEstudiante");
                content.Add(new StringContent(model.CedulaPadreMadre), "CedulaPadreMadre");
                content.Add(new StringContent(model.EdadEstudiante.ToString()), "EdadEstudiante");
                content.Add(new StringContent(model.Direccion), "Direccion");

                if (archivoNotas != null)
                {
                    var fileContent = new StreamContent(archivoNotas.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(archivoNotas.ContentType);
                    content.Add(fileContent, "archivoNotas", archivoNotas.FileName);
                }

                if (archivoCartaFirmada != null)
                {
                    var fileContent = new StreamContent(archivoCartaFirmada.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(archivoCartaFirmada.ContentType);
                    content.Add(fileContent, "archivoCartaFirmada", archivoCartaFirmada.FileName);
                }

                var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Becas/updateBeca/{id}")
                {
                    Content = content
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("MisBecas");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AdminBecas()
        {
            
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
                ViewBag.UserRole = roleClaim?.Value ?? "guest";

                var becas = await _apiService.GetAllBecasAsync();
                if (becas == null)
                {
                    ViewBag.ErrorMessage = "No se pudieron obtener las becas.";
                    return View(new List<BecaResponseModel>());
                }
                return View(becas);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching all becas.";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> MisBecas()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
                ViewBag.UserRole = roleClaim?.Value ?? "guest";

                var becas = await _apiService.GetBecasByUserAsync();
                return View(becas);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching your becas.";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBecaEstado(int id, string estado)
        {
            try
            {
                var actualizarEstadoBeca = await _apiService.UpdateBecaEstadoAsync(id, estado);

                if (actualizarEstadoBeca)
                {
                    return RedirectToAction("AdminBecas");
                }
                else
                {
                    ViewBag.ErrorMessage = "No se pudo actualizar el estado de la beca.";
                    return View("Error");
                }
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Se produjo un error al intentar actualizar el estado de la beca: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBeca(int id)
        {
            try
            {
                var deleteBeca = await _apiService.DeleteBecaAsync(id);
                if (deleteBeca)
                {
                    return RedirectToAction("MisBecas");
                }
                else
                {
                    ViewBag.ErrorMessage = "No se pudo eliminar la beca.";
                    return View("Error");
                }

            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while deleting the beca.";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DescargarReporteBecas()
        {
            try
            {
                var reportBytes = await _apiService.GetBecasReportAsync();

                // Establece el nombre del archivo y el tipo de contenido
                var fileName = "BecasReport.pdf";
                return File(reportBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
    }
}
