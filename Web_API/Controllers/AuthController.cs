using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_API.Data;
using Web_API.Helpers;
using Web_API.Models;

namespace Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHelperBCrypt _passwordHelperBCrypt;
        private readonly JwtTokenHelper _jwtTokenHelper;

        public AuthController(ApplicationDbContext context, PasswordHelperBCrypt passwordHelperBCrypt, JwtTokenHelper jwtTokenHelper)
        {
            _context = context;
            _passwordHelperBCrypt = passwordHelperBCrypt;
            _jwtTokenHelper = jwtTokenHelper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == registerDto.Email && u.Username == registerDto.Username);
                if (existingUser != null)
                {
                    return Conflict("User already exists.");
                }

                var hashedPassword = _passwordHelperBCrypt.HashPassword(registerDto.Password);
                var user = registerDto.ToUser(hashedPassword);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while registering the user.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
                if (user == null || !_passwordHelperBCrypt.VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    return Unauthorized("Invalid username or password.");
                }

                var token = _jwtTokenHelper.GenerateToken(user);

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the login.");
            }
        }

    }
}
