using Microsoft.AspNetCore.Mvc;
using Razor_Front_End.Models;
using Razor_Front_End.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Razor_Front_End.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = await _apiService.GetTokenAsync(model.Username, model.Password);

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            HttpContext.Session.SetString("JwtToken", token);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
            var userRole = roleClaim?.Value ?? "guest";

            // Redirige basado en el rol
            if (userRole == "admin")
            {
                return RedirectToAction("IndexAdmin", "Home");
            }
            else if (userRole == "user")
            {
                return RedirectToAction("IndexUser", "Home");
            }
            else
            {
                // Rol no reconocido, puedes redirigir a una vista de acceso denegado u otra página
                return RedirectToAction("AccessDenied", "Account");
            }

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _apiService.RegisterAsync(model);
                    return RedirectToAction("Login", "Auth");
                }
                catch (HttpRequestException)
                {
                    ModelState.AddModelError(string.Empty, "Registration failed.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Index", "Home");
        }


    }

}
