namespace ApiKey.Authentication;
public interface IApiKeyAuthenticationService
{
    Task<bool> IsValidAsync(string apiKey);
}