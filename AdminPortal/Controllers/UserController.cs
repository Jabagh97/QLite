using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Models.Auth;

namespace AdminPortal.Controllers
{
    [Authorize]

    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public UserController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View("UserPartial");
        }

        private async Task<List<ApplicationUser>> GetUsersFromApi()
        {
            var httpClient = _httpClientFactory.CreateClient("AuthServer");
            httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("AuthBase"));

            var response = await httpClient.GetAsync("Auth/Admin/GetUsers");
            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadAsAsync<List<ApplicationUser>>();
            return users;
        }

        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await GetUsersFromApi();
                return Ok(new { data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(string UserName, string Email, string PasswordHash, AccountType AccountType, Guid Desk, string IsActive)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = UserName,
                    Email = Email,
                    PasswordHash = PasswordHash,
                    AccountType = AccountType,
                    Desk = Desk,
                    IsActive = IsActive.ToLower() == "true" // Convert string to bool
                };

                var httpClient = _httpClientFactory.CreateClient("AuthServer");
                httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("AuthBase"));

                // Include hashed password in the request body
                var response = await httpClient.PostAsJsonAsync("Auth/Admin/CreateUser", user);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errors = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(errorContent);
                    if (errors != null && errors.ContainsKey("errors"))
                    {
                        return BadRequest(new { Errors = errors["errors"] });
                    }
                    else
                    {
                        return BadRequest(new { Errors = new List<string> { "An unknown error occurred." } });
                    }
                }

                return RedirectToAction(nameof(GetUsers));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, ApplicationUser user)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("AuthServer");
                httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("AuthBase"));

                var response = await httpClient.PutAsJsonAsync($"Auth/Admin/UpdateUser/{id}", user);
                response.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(GetUsers));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("AuthServer");
                httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("AuthBase"));

                var response = await httpClient.DeleteAsync($"Auth/Admin/DeleteUser/{id}");
                response.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(GetUsers));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDesks()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("APIServer");
                httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("APIBase"));

                var response = await httpClient.GetAsync("api/Admin/GetAllDesks");
                response.EnsureSuccessStatusCode();

                var desks = await response.Content.ReadAsAsync<List<Desk>>();
                return Json(desks); // Return the desks data as JSON
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


    }
}