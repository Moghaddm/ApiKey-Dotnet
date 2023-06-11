using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using ApiKey.Authentication.Defaults;
using ApiKey.Authentication.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ApiKey.Authentication.Handlers;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private const string AuthorizationHeaderName = "Authorization";
    private const string ApiKeySchemeName = ApiKeyAuthenticationDefaults.AuthenticationScheme;
    private readonly IApiKeyAuthenticationService _authenticationService;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiKeyAuthenticationService authenticationService
    )
        : base(options: options, logger: logger, encoder: encoder, clock: clock) =>
        _authenticationService = authenticationService;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string apiKey;
        bool failed;
        if (!CheckValidHeader(out apiKey, out failed))
            if (failed)
                return AuthenticateResult.Fail("Missing Api-Key Token.");
            else
                return AuthenticateResult.NoResult();

        var isValid = await _authenticationService.IsValidAsync(apiKey);

        if (!isValid)
            return AuthenticateResult.Fail("Invalid Api-Key Token.");

        var claims = new[] { new Claim(ClaimTypes.Name, "Test") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = $"ApiKey \", charset=\"UTF-8\"";
        await base.HandleChallengeAsync(properties);
    }

    private bool CheckValidHeader(out string headerParameter, out bool failed)
    {
        headerParameter = String.Empty;
        failed = false;
        if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            return false;
        else if (
            !AuthenticationHeaderValue.TryParse(
                Request.Headers[AuthorizationHeaderName],
                out AuthenticationHeaderValue headerValue
            )
        )
            return false;
        else if (!ApiKeySchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            return false;
        else if (headerValue.Parameter is null)
        {
            failed = true;
            return false;
        }
        else
        {
            headerParameter = headerValue.Parameter;
            return true;
        }
    }
}
