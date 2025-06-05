using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ExistHelper.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _token = string.Empty; // Initialize to string.Empty

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetToken(string token)
        {
            _token = token;
        }

        public bool IsTokenSet()
        {
            return !string.IsNullOrWhiteSpace(_token);
        }

        public async Task<string> GetApiResultsAsync(string endpoint, Dictionary<string, string>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(_token))
                return string.Empty;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _token);

            string url = endpoint;
            if (parameters != null && parameters.Count > 0)
            {
                var query = new List<string>();
                foreach (var kvp in parameters)
                {
                    query.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
                }
                url += (endpoint.Contains("?") ? "&" : "?") + string.Join("&", query);
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetTokenFromCredentialsAsync(string username, string password)
        {
            var payload = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };
            var content = new FormUrlEncodedContent(payload);

            var response = await _httpClient.PostAsync("https://exist.io/api/2/auth/simple-token/", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("token", out var tokenElement))
            {
                return tokenElement.GetString() ?? string.Empty;
            }
            return string.Empty;
        }
    }
}