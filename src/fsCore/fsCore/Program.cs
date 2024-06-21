using Common;
using Common.Authentication;
using fsCore.Service;
using fsCore.Middleware;
using fsCore.Service.Hangfire;
using fsCore.Service.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System.Text.Json;
using DataImporter;
using Microsoft.Net.Http.Headers;
using Common.Models.Validators;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);


var config = builder.Configuration;
var environment = builder.Environment;

var dbConnectString = config.GetConnectionString("DefaultConnection");
var clientId = config["ClientConfig:AuthorityClientId"];
var issuerHost = config["JWT_ISSUER_HOST"];
var authAudience = config["JWT_AUDIENCE"];
var useStaticFiles = config["UseStaticFiles"];

if (string.IsNullOrEmpty(useStaticFiles) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(issuerHost) || string.IsNullOrEmpty(authAudience) || string.IsNullOrEmpty(dbConnectString))
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
    .AddModelValidators();

builder.Services
    .AddSqlPersistence(config);

builder.Services
    .AddDataImporter(config, environment);

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
        options.Authority = issuerHost;
        options.RequireHttpsMetadata = !environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false,
            ValidIssuer = issuerHost
        };
    });

builder.Services
    .AddHttpClient<IUserInfoClient, UserInfoClient>();

builder.Services
    .AddScoped<IWorldFishService, WorldFishService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IGroupService, GroupService>()
    .AddScoped<IGroupCatchService, GroupCatchService>()
    .AddScoped<IHangfireJobsService, HangfireJobService>();

builder.Services
    .AddHangfire(configuration => configuration?
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        ?.UseSimpleAssemblyNameTypeSerializer()
        ?.UseRecommendedSerializerSettings()
        ?.UsePostgreSqlStorage(dbConnectString))
        ?.AddHangfireServer(options =>
        {
            options.Queues = HangfireConstants.Queues.FullList;
        });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var hangfireService = scope.ServiceProvider.GetRequiredService<IHangfireJobsService>();
    hangfireService.RegisterJobs();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("corsapp");
}
else
{
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseResponseCompression();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseDefaultMiddlewares();
app.MapControllers();
#pragma warning disable ASP0014
app.UseEndpoints(endpoint =>
{
    endpoint.MapFallbackToFile("index.html");
});
#pragma warning restore ASP0014 
if (bool.Parse(useStaticFiles) is true)
{

    app.UseStaticFiles();
    app.UseSpa(spa =>
    {
        spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
        {
            OnPrepareResponse = context =>
            {
                var headers = context.Context.Response.GetTypedHeaders();
                if (context.File.Name.EndsWith(".html"))
                {
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true,
                        NoStore = true,
                        MustRevalidate = true,
                        MaxAge = TimeSpan.Zero
                    };
                }
                else
                {
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        Private = false,
                        NoCache = false,
                        NoStore = false,
                        MaxAge = TimeSpan.FromDays(365)
                    };
                }
            }
        };
    });
}

app.Run();
public static partial class Program { };