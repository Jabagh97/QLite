using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLite.Data.Models.Auth;
using QLiteAuthenticationServer.Context;
using QLiteAuthenticationServer.Helpers;
using System.Security.Claims;

namespace QLiteAuthenticationServer.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("Auth/Admin/GetUsers")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }
        [HttpPost]
        [Route("Auth/Admin/CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] ApplicationUser user)
        {
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest("User object or hashed password is null or empty");
            }

            try
            {

                user.EmailConfirmed= true;
                user.TwoFactorEnabled= false;
                user.QRless= true;
                user.TwoFactorSecret= Utils.GenerateTwoFactorKey();
               

                // Create the user in the database
                var result = await _userManager.CreateAsync(user,user.PasswordHash);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new { Errors = errors });
                }
                // Add additional claims to the user's identity
                var claims = new Claim[] { new Claim("DeskId", user.Desk.ToString()), new Claim("AccountType",user.AccountType.ToString()) };

                result =  _userManager.AddClaimsAsync(user, claims).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
               

              

                // Return the created user
                return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the user: {ex.Message}");
            }
        }





        [HttpPut]
        [Route("Auth/Admin/UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUser user)
        {
            if (id != user.Id)
            {
                return BadRequest("User ID mismatch");
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound("User not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete]
        [Route("Auth/Admin/DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}