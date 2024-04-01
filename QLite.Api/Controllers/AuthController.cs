using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QLiteDataApi.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace QLiteDataApi.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        public AuthController()
        {
        }

        [HttpGet("GenerateToken")]
        public IActionResult GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiContext.Config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(ApiContext.Config["Jwt:Issuer"],
              ApiContext.Config["Jwt:Audience"],
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
