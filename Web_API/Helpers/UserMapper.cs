using Web_API.Models;

namespace Web_API.Helpers
{
    public static class UserMapper
    {
        public static User ToUser(this UserRegisterDto registerDto, string hashedPassword)
        {
            return new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                Cedula = CleanCedula(registerDto.Cedula),
                Nombre = registerDto.Nombre,
                Apellidos = registerDto.Apellidos,
                PasswordHash = hashedPassword,
                Role = "user"
            };
        }

        private static string CleanCedula(string cedula)
        {
            return new string(cedula.Where(char.IsDigit).ToArray());
        }
    }
}
