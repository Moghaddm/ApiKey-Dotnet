using ApiKey.Authentication;

namespace ApiKey.Services;

public class ApiKeyAuthenticationService : IApiKeyAuthenticationService
{
    public Task<bool> IsValidAsync(string apiKey)
    {
        // Set Your Logic For KEY !
        return Task.FromResult(apiKey == "test");
    }
}
