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

// Just Add Your Scheme !
builder.Services.AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
  .AddApiKey<ApiKeyAuthenticationService>();

builder.Services.AddSingleton<IApiKeyAuthenticationService,ApiKeyAuthenticationService>();

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
