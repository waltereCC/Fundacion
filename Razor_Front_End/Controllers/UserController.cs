using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Razor_Front_End.Models;
using Razor_Front_End.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Razor_Front_End.Controllers
{
    public class UserController : Controller
    {

        private readonly HttpClient _client;
        private readonly ApiService _apiService;

        public UserController(IHttpClientFactory httpClientFactory, ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.UserRole = "guest";
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
            var users = await _apiService.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(int userId, string newRole)
        {

            var result = await _apiService.UpdateUserRoleAsync(userId, newRole);
            if (result)
            {
                return RedirectToAction("ObtenerUsuarios");
            }

            ViewBag.ErrorMessage = "Failed to update user role.";
            return View("ObtenerUsuarios");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var userId = GetUserIdFromToken();
            var user = await _apiService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserUpdateViewModel
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellidos = user.Apellidos,
                Email = user.Email,
                Cedula = user.Cedula,
                Username = user.Username
            };

            return View(); 
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }
    }
}
