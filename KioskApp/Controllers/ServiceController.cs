using Microsoft.AspNetCore.Mvc;
using QLite.Data;
using QLite.Data.Dtos;
using System.ComponentModel.DataAnnotations;
using KioskApp.Constants;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace KioskApp.Controllers
{
public class ServiceController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ServiceController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            // Set base address for HttpClient
            _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("APIBase"));
        }

        public async Task<IActionResult> Index(Guid segmentOid)
        {
            try
            {
                var services = await FetchServiceTypesFromAPIAsync(segmentOid);

                ViewBag.Services = services;

                return PartialView("Views/Home/Services.cshtml");
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<List<ServiceType>> FetchServiceTypesFromAPIAsync(Guid segmentOid)
        {
            var response = await _httpClient.GetAsync($"{EndPoints.GetServiceTypeList}?segmentId={segmentOid}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var services = JsonConvert.DeserializeObject<List<ServiceType>>(json);

            return services;
        }

        [HttpPost]
        public async Task<IActionResult> GetTicket([FromBody] TicketRequestDto ticketRequest)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(ticketRequest), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(EndPoints.GetTicket, content);
                response.EnsureSuccessStatusCode();

                var ticketJson = await response.Content.ReadAsStringAsync();
                var ticket = JsonConvert.DeserializeObject<TicketDto>(ticketJson);

                return Ok(ticket);
            }
            catch (HttpRequestException ex)
            {
                // Log or handle specific HTTP request exceptions
                return StatusCode(500, $"HTTP request error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                // Log or handle JSON parsing exceptions
                return StatusCode(500, $"JSON parsing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }}
