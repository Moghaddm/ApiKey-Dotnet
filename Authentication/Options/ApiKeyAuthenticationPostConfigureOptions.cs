using ApiKey.Authentication.Options;
using Microsoft.Extensions.Options;

namespace ApiKet.Authentication.Options;
// Configuring Post Requests
public class ApiKeyAuthenticationPostConfigureOptions
    : IPostConfigureOptions<ApiKeyAuthenticationOptions>
{
    public void PostConfigure(string name, ApiKeyAuthenticationOptions options) { }
}
