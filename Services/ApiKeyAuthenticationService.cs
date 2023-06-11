using ApiKey.Authentication;

namespace ApiKey.Services;

public class ApiKeyAuthenticationService : IApiKeyAuthenticationService
{
    public Task<bool> IsValidAsync(string apiKey)
    {
        return Task.FromResult(apiKey == "test");
    }
}
