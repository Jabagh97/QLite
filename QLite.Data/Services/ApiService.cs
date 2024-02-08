using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace QLite.Data.Services
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestData);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode(); // Ensure successful response

            // Deserialize response content to specified type T
            var responseData = await response.Content.ReadAsAsync<T>();
            return responseData;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestData)
        {
            // Serialize request data
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            // Make POST request to API endpoint
            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode(); // Ensure successful response

            // Deserialize response content to specified type TResponse
            var responseData = await response.Content.ReadAsAsync<TResponse>();
            return responseData;
        }
    }
}