using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using ApiKey.Authentication.Defaults;
using ApiKey.Authentication.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;

namespace ApiKey.Authentication.Handlers;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private const string AuthorizationHeaderName = "Authorization";
    private const string ApiKeySchemeName = ApiKeyAuthenticationDefaults.AuthenticationScheme;
    private readonly IApiKeyAuthenticationService _authenticationService;

    /// <summary>
    /// All Of Dependencies For Authentication Handler
    /// </summary>
    /// <param name="options">Monitoring And Seeing Changes of Auth Instance</param>
    /// <param name="logger">Just Logging Changes</param>
    /// <param name="encoder">Encoding Auth Informations</param>
    /// <param name="clock">Timing</param>
    /// <param name="authenticationService">Main Defined Service For Check Requests With Abstract</param>
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiKeyAuthenticationService authenticationService
    )
        : base(options: options, logger: logger, encoder: encoder, clock: clock) =>
        _authenticationService = authenticationService;
    /// <summary>
    /// Main Handle For Auth Header
    /// </summary>
    /// <returns></returns>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        bool failed;
        string parameter;
        if (!CheckValidHeader(out parameter, out failed))
            if (failed)
                return AuthenticateResult.Fail("Missing Api-Key Token.");
            else
                return AuthenticateResult.NoResult();

        var isValid = await _authenticationService.IsValidAsync(parameter);

        if (!isValid)
            return AuthenticateResult.Fail("Invalid Api-Key Token.");

        var claims = new[] { new Claim(ClaimTypes.Name, "Test") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        return AuthenticateResult.Success(ticket);
    }
    /// <summary>
    /// Check When Request is 401 and its Possible To Modify that!
    /// </summary>
    /// <param name="properties">Auth Failed Props</param>
    /// <returns></returns>
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = $"ApiKey , charset=\"UTF-8\"";
        await base.HandleChallengeAsync(properties);
    }
    /// <summary>
    /// Just Check The Header May Sent
    /// </summary>
    /// <param name="headerParameter">header Value Sended</param>
    /// <param name="failed">Check The Operation is Failed?</param>
    /// <returns></returns>
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
