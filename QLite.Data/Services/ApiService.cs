using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace QLite.Data.Services
{
    public interface IApiService
    {
        Task<HttpResponseMessage> GetAsync<TRequest>(string endpoint, TRequest requestData = default);
        Task<HttpResponseMessage> PostAsync<TRequest>(string endpoint, TRequest requestData);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;


        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("APIBase"));
        }

        public async Task<HttpResponseMessage> GetAsync<TRequest>(string endpoint, TRequest requestData = default)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode(); // Ensure successful response

            return response;
        }

        public async Task<HttpResponseMessage> PostAsync<TRequest>(string endpoint, TRequest requestData)
        {
            // Serialize request data
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            // Make POST request to API endpoint
            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode(); // Ensure successful response

            return response;
        }
    }
}