using Microsoft.AspNetCore.Mvc;
using OrdinarioAPI.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrdinarioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://rickandmortyapi.com/api/character";

        public CharacterController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Character>>> GetCharacters()
        {
            try
            {
                // Realiza la solicitud HTTP
                var response = await _httpClient.GetAsync(BaseUrl);
                response.EnsureSuccessStatusCode();

                // Obtén el contenido de la respuesta
                var content = await response.Content.ReadAsStringAsync();

                // Deserializa el contenido en el modelo ApiResponse<Character>
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<Character>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Extrae los personajes de la propiedad Results
                var characters = apiResponse?.Results;

                // Devuelve los personajes como resultado
                return Ok(characters);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error al conectar con la API externa: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"Error al deserializar la respuesta de la API: {ex.Message}");
            }
        }

        // GET: api/Character/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Character>> GetCharacter(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound($"Character con ID {id} no encontrado.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var character = JsonSerializer.Deserialize<Character>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Ok(character);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error al conectar con la API externa: {ex.Message}");
            }
        }
    }
}



