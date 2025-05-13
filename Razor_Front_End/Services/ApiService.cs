using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Razor_Front_End.Models;

namespace Razor_Front_End.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _client = httpClientFactory.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            var response = await _client.PostAsync("/api/auth/login", new StringContent(
                JsonConvert.SerializeObject(new { username, password }),
                Encoding.UTF8,
                "application/json"
            ));

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
                return tokenResponse?.Token;
            }

            return null;
        }

        public async Task RegisterAsync(RegisterViewModel model)
        {
            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/register", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error registering user: {errorResponse}");
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/User/getAllUsers");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<User>>();
            }

            return null;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"/api/User/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }

            return null;
        }

        public async Task<bool> UpdateUserAsync(UserUpdateViewModel model)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync($"/api/User/{model.Id}", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, string newRole)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new UpdateUserRoleRequest
            {
                UserId = userId,
                NewRole = newRole
            };

            var response = await _client.PutAsJsonAsync("/api/User/updateUserRole", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<UserBecasModel> GetUserDataAsync()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("No JWT token found.");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/usuarios/obtenerDatos");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserBecasModel>(responseBody);
            }

            throw new HttpRequestException("Failed to retrieve user data.");
        }

        public async Task<bool> SolicitarBeca(BecaViewModel model)
        {
            using (var content = new MultipartFormDataContent())
            {
                // Add form data
                content.Add(new StringContent(model.NombrePadreMadre), "NombrePadreMadre");
                content.Add(new StringContent(model.NombreEstudiante), "NombreEstudiante");
                content.Add(new StringContent(model.CedulaPadreMadre), "CedulaPadreMadre");
                content.Add(new StringContent(model.EdadEstudiante.ToString()), "EdadEstudiante");
                content.Add(new StringContent(model.Direccion), "Direccion");
                content.Add(new StringContent(model.Estado), "Estado");

                // Add files
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

                var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _client.PostAsync("/api/becas", content);

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<List<BecaResponseModel>> GetBecasByUserAsync()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/becas/getBecasByUser");

            if (response.IsSuccessStatusCode)
            {
                var becas = await response.Content.ReadFromJsonAsync<List<BecaResponseModel>>();
                return becas;
            }

            return new List<BecaResponseModel>();
        }

        public async Task<List<BecaResponseModel>> GetAllBecasAsync()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/Becas/getAllBecas");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<BecaResponseModel>>();
            }
            return null;
        }

        public async Task<bool> UpdateBecaEstadoAsync(int becaId, string nuevoEstado)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("No se encontró el token de autorización.");
            }

            var updateRequest = new UpdateEstadoBecaRequest
            {
                Id = becaId,
                Estado = nuevoEstado
            };

            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Becas/updateEstado/{becaId}");
            request.Content = new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBecaAsync(int becaId)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("No se encontró el token de autorización.");
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/Becas/deleteBeca/{becaId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public async Task<byte[]> GetBecasReportAsync()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/Becas/generateBecasReport");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                // Manejo de errores, por ejemplo, lanzar una excepción o retornar un valor por defecto
                throw new Exception("No se pudo obtener el informe.");
            }
        }

        public class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}
