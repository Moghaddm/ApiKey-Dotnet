using ApiKey.Authentication;
using ApiKey.Authentication.Defaults;
using ApiKey.Authentication.Extensions;
using ApiKey.Authentication.Handlers;
using ApiKey.Authentication.Options;
using ApiKey.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
  //.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationDefaults.AuthenticationScheme, null)
  .AddApiKey<ApiKeyAuthenticationService>();

builder.Services.AddSingleton<IApiKeyAuthenticationService,ApiKeyAuthenticationService>();

// builder.Services
//     .AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
//     .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
//         ApiKeyAuthenticationDefaults.AuthenticationScheme,
//         "ApiKey",
//         options => { }
//     );
    // .AddApiKey<ApiKeyAuthenticationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

// app.UseAuthorization();

app.MapControllers();

app.Run();
