using Common;
using Common.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);


var config = builder.Configuration;
var enviroment = builder.Environment;

var authOptions = config.GetSection(AuthoritySettings.Key).Get<AuthoritySettings>();
var clientId = config["ClientConfig:AuthorityClientId"];
var issuerHost = config["JWT_ISSUER_HOST"];
var authAudience = config["JWT_AUDIENCE"];


if(string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(issuerHost) || string.IsNullOrEmpty(authAudience))
{
    throw new Exception(ErrorConstants.MissingEnvVars);
}


builder.Services
    .AddHttpContextAccessor()
    .AddResponseCompression()
    .AddLogging()
    .AddHttpClient()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

builder.Services
    .AddAuthorization();

builder.Services
    .Configure<AuthoritySettings>(config.GetSection(AuthoritySettings.Key))
    .Configure<ClientConfigSettings>(config.GetSection(ClientConfigSettings.Key));

builder.Services
                .AddScoped<IAuthenticationStrategy, BearerTokenAuthenticationStrategy>()
                .AddSingleton<IBearerTokenAuthenticationStrategy, BearerTokenAuthenticationStrategy>();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authOptions.Host;
        options.RequireHttpsMetadata = !enviroment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuers = issuerHost.Split(","),
            ValidAudience = authAudience
        };
        options.ForwardDefaultSelector = (context) => JwtBearerDefaults.AuthenticationScheme;
    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("corsapp");
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public static partial class Program { };