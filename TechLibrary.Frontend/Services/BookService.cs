using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using TechLibrary.Frontend.Models;

namespace TechLibrary.Frontend.Services
{
    public class BookService
    {
        private readonly HttpClient _httpClient;

        public BookService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BookResponse> GetBooksAsync(int pageNumber, int pageSize)
        {
            var response = await _httpClient.GetAsync($"Books/Filter?pageNumber={pageNumber}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BookResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task ReserveBookAsync(Guid bookId)
        {
            var response = await _httpClient.PostAsync($"/Checkouts/{bookId}", null);
            response.EnsureSuccessStatusCode();
        }
    }
}