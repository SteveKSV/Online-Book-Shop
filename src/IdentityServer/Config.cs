using IdentityServer4.Models;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]{
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> {"role"}
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
          new[] {
                new ApiScope("Catalog.read"),
                new ApiScope("Catalog.write"),
                new ApiScope("Basket.read"),
                new ApiScope("Basket.write"),
                new ApiScope("Discount.read"),
                new ApiScope("Discount.write"),
                new ApiScope("Ordering.read"),
                new ApiScope("Ordering.write"),
          };

        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("Catalog")
                {
                    Scopes = new List<string> { "Catalog.read", "Catalog.write" },
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = new List<string> { "role" }
                },  
                new ApiResource("Basket")
                {
                    Scopes = new List<string> { "Basket.read", "Basket.write" },
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = new List<string> { "role" }
                },
                new ApiResource("Discount")
                {
                    Scopes = new List<string> { "Discount.read", "Discount.write" },
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = new List<string> { "role" }
                },
                new ApiResource("Ordering")
                {
                    Scopes = new List<string> { "Ordering.read", "Ordering.write" },
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = new List<string> { "role" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                    AllowedScopes = { "Catalog.read", "Catalog.write", "Basket.read", "Discount.read", "Ordering.read" }
                },
                new Client
                {
                    ClientId = "interactive",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                    AllowedScopes = { "openid", "profile", "Catalog.read" },
                    RedirectUris = { "https://localhost:5007/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:5007/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:5007/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    RequirePkce = true,
                    RequireConsent = true,
                    AllowPlainTextPkce = false
                }
            };
    }
}