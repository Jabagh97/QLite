using Newtonsoft.Json;
using QLite.Dto.Kapp;
using Quavis.QorchLite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Quavis.QorchLite.Common
{
    public class HttpCaller
    {
        HttpClient _http;

        public HttpCaller(HttpClient http)
        {
            if (http == null)
            {
                _http = new HttpClient() { BaseAddress = new Uri("http://localhost:5000/") };
            }
            else
                _http = http;
        }


        public async Task GetHttpReqAsync(string query, string token)
        {
            LoggerAdapter.Debug("GetHttpReqAsync:" + query);

            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await HandleServiceErrors(async () =>
            {
                return await _http.GetAsync(query);
            });
        }


       

        public async Task<T> GetHttpReqAsync<T>(string query, string token)
        {
            LoggerAdapter.Debug("GetHttpReqAsync:" + query);

            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var resp = await HandleServiceErrors(async () =>
            {
                return await _http.GetAsync(query);
            });

            if (resp == null)
            {
                return default(T);
            }
            var content = await resp.Content.ReadAsStringAsync();

            try
            {
                if (typeof(T).IsValueType || typeof(T).Name == typeof(string).Name)
                    return (T)Convert.ChangeType(content, typeof(T));
                var resp2 = JsonConvert.DeserializeObject<T>(content);
                return resp2;
            }
            catch (Exception ex)
            {
                LoggerAdapter.Error("deserialization error:" + ex.Message);
                LoggerAdapter.Error("content:" + content);
                throw;
            }
        }

        public async Task<T> PostHttpReqAsync<T>(string query, object data, string token)
        {

            var resp = await HandleServiceErrors(async () =>
            {
                var par = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                if (!string.IsNullOrEmpty(token))
                    _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                return await _http.PostAsync(query, par);
            });

            if (resp == null)
                return default(T);

            var content = await resp.Content.ReadAsStringAsync();

            try
            {
                if (typeof(T).IsValueType || typeof(T).Name == typeof(string).Name)
                    return (T)Convert.ChangeType(content, typeof(T));
                var resp2 = JsonConvert.DeserializeObject<T>(content);
                return resp2;

            }
            catch (Exception ex)
            {
                LoggerAdapter.Error("deserialization error:" + ex.Message);
                LoggerAdapter.Error("content:" + content);
                throw;
            }
        }


        private static async Task<HttpResponseMessage> HandleServiceErrors(Func<Task<HttpResponseMessage>> func)
        {
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await func.Invoke();
            }
            catch (Exception ex) // burada exception olursa servise gitmedik demektir zira servise gidebilsek içerden exception sızdırmıyoruz dışarı. 
            {
                LoggerAdapter.Error(ex);
                return null;
            }

            var content = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                var dse = GetDataServiceError(content);
                return null;  // servisten gelen hata bilgisine ihtiyaç olan bir durum olursa exception atabiliriz

            }
            return httpResponse;
        }

        private static HttpResponseMessage HandleServiceErrors(Func<HttpResponseMessage> func)
        {
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = func.Invoke();
            }
            catch (Exception ex) // burada exception olursa servise gitmedik demektir zira servise gidebilsek içerden exception sızdırmıyoruz dışarı. 
            {
                LoggerAdapter.Error(ex);
                return null;
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                //var dse = GetDataServiceError(content);
                return null;  // servisten gelen hata bilgisine ihtiyaç olan bir durum olursa exception atabiliriz

            }
            return httpResponse;
        }

        public static KappServiceErrorDto GetDataServiceError(string content)
        {
            LoggerAdapter.Debug("GetDataServiceError: " + content);
            try
            {
                var dse = JsonConvert.DeserializeObject<KappServiceErrorDto>(content);
                return dse;
            }
            catch (Exception ex)
            {
                LoggerAdapter.Error(ex);
                return null;
            }
        }

    }
}
