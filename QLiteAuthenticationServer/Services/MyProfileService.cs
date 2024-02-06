using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using QLite.Data.Models.Auth;
using QLiteAuthenticationServer.Context;
using System.Security.Claims;

namespace QLiteAuthenticationServer.Services
{
    public class MyProfileService : IProfileService
    {
        protected UserManager<ApplicationUser> _userManager;
        protected ApplicationDbContext _context;

        public MyProfileService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// user to manually inject desired claims
        /// </summary>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //>Processing
            var user = await _userManager.GetUserAsync(context.Subject);

            if (user != null)
            {
                var claims = _context.UserClaims
                            .Where(x => x.UserId == user.Id)
                            .Select(x => new Claim(x.ClaimType, x.ClaimValue))
                            .ToList();

                claims.Add(new Claim("Name", user.UserName));

              

                context.IssuedClaims.AddRange(claims.AsEnumerable());
                context.IssuedClaims.RemoveAll(c =>
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" // we already have an email claim
                    || c.Type == "preferred_username" // we already have a name claim
                    || c.Type == "amr" // Authentication method reference. It tells you how the user authenticated (e.g., pwd means password). Useful for some scenarios.
                    || c.Type == "idp" // Identity provider. Indicates how the user authenticated (e.g., local means they authenticated directly with your server). Useful for some scenarios.
                );

            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            //>Processing
            var user = await _userManager.GetUserAsync(context.Subject);

            context.IsActive = (user != null) && user.IsActive;
        }

        public IEnumerable<Claim> GetClaims(ClaimsIdentity identity)
        {
            var claims = new List<Claim>();

            // Include only the necessary claims
            claims.Add(new Claim("sub", identity.FindFirst("sub").Value));
            claims.Add(new Claim("email", identity.FindFirst("email").Value));

            return claims;
        }

    }

}
