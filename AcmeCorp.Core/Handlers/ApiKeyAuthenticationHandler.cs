using AcmeCorp.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AcmeCorp.Core.Handlers
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IApiKeyService _apiKeyService;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IApiKeyService apiKeyService)
            : base(options, logger, encoder, clock)
        {
            _apiKeyService = apiKeyService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var apiKeyHeaderValues))
            {
                return AuthenticateResult.Fail("API key not found in the request header");
            }

            var apiKey = apiKeyHeaderValues.ToString();

            var isValidApiKey = await _apiKeyService.ValidateApiKeyAsync(apiKey);

            if (!isValidApiKey)
            {
                return AuthenticateResult.Fail("Invalid API key");
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "APIKeyUser") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }

}