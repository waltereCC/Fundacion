using Microsoft.AspNetCore.Mvc;
using Razor_Front_End.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Razor_Front_End.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Donaciones()
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
            return View();
        }

        public IActionResult Index()
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

            return View();
        }

        public IActionResult IndexUser()
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
            return View();
        }

        public IActionResult IndexAdmin()
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
            return View();
        }

        public IActionResult QuienesSomos()
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
            return View(); }
    }
}
