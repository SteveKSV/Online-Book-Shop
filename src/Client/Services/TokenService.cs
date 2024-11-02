using Client.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace Client.Services
{
    public class TokenService : ITokenService
    {
        public readonly IOptions<IdentityServerSettings> identityServerSettings;
        public readonly DiscoveryDocumentResponse discoveryDocument;
        private readonly HttpClient httpClient;

        public TokenService(IOptions<IdentityServerSettings> identityServerSettings)
        {
            this.identityServerSettings = identityServerSettings;
            httpClient = new HttpClient();

            // Get the DiscoveryUrl from the settings
            var discoveryUrl = this.identityServerSettings.Value.DiscoveryUrl;

            // Create the base Uri from the discovery URL
            var discoveryBaseUri = new Uri(discoveryUrl);

            // Construct the discovery document URI explicitly
            string discoveryFinalUriString = $"{discoveryBaseUri.Scheme}://{discoveryBaseUri.Host}:{discoveryBaseUri.Port}/.well-known/openid-configuration";
            var discoveryFinalUri = new Uri(discoveryFinalUriString);

            Console.WriteLine($"Discovery URL: {discoveryUrl}");
            Console.WriteLine($"Discovery Final URI: {discoveryFinalUri}");

            // Now create the DiscoveryDocumentRequest
            var discoveryRequest = new DiscoveryDocumentRequest
            {
                Address = discoveryFinalUri.ToString(), // Use the full discovery document URL
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = this.identityServerSettings.Value.RequireHttpsMetadata
                },
                RequestUri = discoveryFinalUri // Use the full URI here as well
            };

            try
            {
                // Perform the discovery document request
                discoveryDocument = httpClient.GetDiscoveryDocumentAsync(discoveryRequest).Result;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get discovery document", ex);
            }

            if (discoveryDocument.IsError)
            {
                throw new Exception("Unable to get discovery document", discoveryDocument.Exception);
            }
        }


        public async Task<TokenResponse> GetToken(string scope)
        {
            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = identityServerSettings.Value.ClientName,
                    ClientSecret = identityServerSettings.Value.ClientPassword,
                    Scope = scope
                });

            if (tokenResponse.IsError)
            {
                throw new Exception("Unable to get token", tokenResponse.Exception);
            }

            return tokenResponse;
        }
    }
}
