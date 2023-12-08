using Common;
using Common.Authentication;
using fsCore.Contexts;
using fsCore.Middleware;
using fsCore.Service;
using fsCore.Service.Hangfire;
using fsCore.Service.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);


var config = builder.Configuration;
var environment = builder.Environment;

var authOptions = config.GetSection(AuthoritySettings.Key).Get<AuthoritySettings>();
var dbConnectString = config.GetConnectionString("DefaultConnection");
var clientId = config["ClientConfig:AuthorityClientId"];
var issuerHost = config["JWT_ISSUER_HOST"];
var authAudience = config["JWT_AUDIENCE"];


if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(issuerHost) || string.IsNullOrEmpty(authAudience) || string.IsNullOrEmpty(dbConnectString))
{
    throw new Exception(ErrorConstants.MissingEnvVars);
}

builder.Services
    .AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    })
    .AddDistributedMemoryCache()
    .AddHttpContextAccessor()
    .AddResponseCompression()
    .AddLogging()
    .AddHttpClient()
    .AddLogging()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

builder.Services
    .AddSqlPersistence(config);

builder.Services
    .AddAuthorization()
    .Configure<AuthoritySettings>(config.GetSection(AuthoritySettings.Key))
    .Configure<ClientConfigSettings>(config.GetSection(ClientConfigSettings.Key));

builder.Services
                .AddScoped<IAuthenticationStrategy, BearerTokenAuthenticationStrategy>()
                .AddSingleton<IBearerTokenAuthenticationStrategy, BearerTokenAuthenticationStrategy>();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authOptions?.Host;
        options.RequireHttpsMetadata = !environment.IsDevelopment();
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

builder.Services
    .AddHttpClient<IUserInfoClient, UserInfoClient>();

builder.Services
    .AddScoped<IWorldFishService, WorldFishService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IGroupService, GroupService>()
    .AddScoped<IHangfireJobsService, HangfireJobService>();

builder.Services
    .AddHangfire(configuration => configuration?
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        ?.UseSimpleAssemblyNameTypeSerializer()
        ?.UseRecommendedSerializerSettings()
        ?.UsePostgreSqlStorage(dbConnectString))
        ?.AddHangfireServer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var hangfireService = scope.ServiceProvider.GetRequiredService<IHangfireJobsService>();
    hangfireService.RegisterRecurringJobs();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("corsapp");
}

app.UseSession();
app.UseHttpsRedirection();
app.UseHttpLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseDefaultMiddlewares();
app.Run();

public static partial class Program { };