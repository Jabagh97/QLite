using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QLite.Data.CommonContext;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri(CommonCtx.Config.GetValue<string>("APIBase"));

        }

        public async Task<T> GetDesignResponse<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseData);
            }
            else
            {
                return default;
            }
        }
        public async Task<T> GetGenericResponse<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    T deserializedData = JsonConvert.DeserializeObject<T>(responseData);
                    return deserializedData;
                }
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public async Task<T> PostGenericRequest<T>(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var deserializedData = JsonConvert.DeserializeObject<T>(responseData);
                    return deserializedData;
                }
                else
                {
                    return default; // or throw an appropriate exception
                }
            }
            catch (Exception ex)
            {
                throw ex; // Rethrow the exception to be handled in the controller
            }
        }




    }
}
