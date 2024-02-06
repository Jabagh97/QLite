using IdentityServer4.Models;
using IdentityServer4;

namespace QLiteAuthenticationServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My API")
            };

        public static IEnumerable<Client> Clients(IConfiguration configuration) =>
            new List<Client>
            {
                // Admin Portal
                new Client
                {
                    ClientId = configuration["Clients:AdminPortal:ClientId"],
                    ClientName = configuration["Clients:AdminPortal:ClientName"],

                    ClientSecrets = { new Secret(configuration["Clients:AdminPortal:ClientSecret"].Sha256()) },

                    AllowPlainTextPkce = false,
                    RequirePkce = true,

                    // `id_token` flow matches `implicit` gramt type
                    AllowedGrantTypes = GrantTypes.Implicit,

                    // where to redirect to after login
                    RedirectUris = { configuration["Clients:AdminPortal:SiteDomain"] + "/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { configuration["Clients:AdminPortal:SiteDomain"] + "/signout-callback-oidc" },

                    FrontChannelLogoutUri = configuration["Clients:AdminPortal:SiteDomain"] + "/signout-callback-oidc",

                    AllowOfflineAccess = false, // used to control whether the client is allowed to request refresh tokens without requiring the user to be present for authentication again.

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },

                // Desk App
                new Client
                {
                    ClientId = configuration["Clients:DeskApp:ClientId"],
                    ClientName = configuration["Clients:DeskApp:ClientName"],

                    ClientSecrets = { new Secret(configuration["Clients:DeskApp:ClientSecret"].Sha256()) },

                    AllowPlainTextPkce = false,
                    RequirePkce = true,

                    // `id_token` flow matches `implicit` gramt type
                    AllowedGrantTypes = GrantTypes.Implicit,

                    // where to redirect to after login
                    RedirectUris = { configuration["Clients:DeskApp:SiteDomain"] + "/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { configuration["Clients:DeskApp:SiteDomain"] + "/signout-callback-oidc" },

                    FrontChannelLogoutUri = configuration["Clients:DeskApp:SiteDomain"] + "/signout-callback-oidc",

                    AllowOfflineAccess = false, // used to control whether the client is allowed to request refresh tokens without requiring the user to be present for authentication again.

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
            };
    }

}
