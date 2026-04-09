using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace RVM.McpForge.API.Auth;

public class ApiKeyAuthHandler(
    IOptionsMonitor<ApiKeyAuthOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<ApiKeyAuthOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Api-Key", out var apiKeyHeaderValues))
            return Task.FromResult(AuthenticateResult.NoResult());

        var providedKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrEmpty(providedKey))
            return Task.FromResult(AuthenticateResult.Fail("Empty API key."));

        var matchedKey = Options.Keys.FirstOrDefault(k => k.Key == providedKey);
        if (matchedKey is null)
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, matchedKey.AppId),
            new Claim("AppId", matchedKey.AppId)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        Context.Items["AppId"] = matchedKey.AppId;

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
