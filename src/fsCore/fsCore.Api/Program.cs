using System.Text.Json;
using fsCore.Api.Hubs;
using fsCore.Api.Middleware;
using fsCore.Common.Authentication;
using fsCore.Common.Misc;
using fsCore.Common.Models.Validators;
using fsCore.DataImporter;
using fsCore.Persistence;
using fsCore.Services;
using fsCore.Services.Abstract;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

var config = builder.Configuration;
var environment = builder.Environment;

var dbConnectString = config.GetConnectionString("DefaultConnection");
var clientId = config.GetSection("ClientConfig").GetSection("AuthorityClientId")?.Value;
var issuerHost = config.GetSection("JWT_ISSUER_HOST")?.Value;
var authAudience = config.GetSection("JWT_AUDIENCE")?.Value;
var useStaticFiles = config.GetSection("UseStaticFiles")?.Value;

if (
    string.IsNullOrEmpty(useStaticFiles)
    || string.IsNullOrEmpty(clientId)
    || string.IsNullOrEmpty(issuerHost)
    || string.IsNullOrEmpty(authAudience)
    || string.IsNullOrEmpty(dbConnectString)
)
{
    throw new Exception(ErrorConstants.MissingEnvVars);
}

builder
    .Services.AddDistributedMemoryCache()
    .AddHttpContextAccessor()
    .AddResponseCompression()
    .AddRequestTimeouts(opts =>
    {
        opts.DefaultPolicy = new RequestTimeoutPolicy { Timeout = TimeSpan.FromMilliseconds(5000) };
    })
    .AddLogging()
    .AddHttpClient()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    );

builder.Services.AddModelValidators();

builder.Services.AddSqlPersistence(config);

builder.Services.AddDataImporter(config, environment);

builder
    .Services.AddAuthorization()
    .Configure<ClientConfigSettings>(config.GetSection(ClientConfigSettings.Key));

builder.Services.AddSignalRFsCore();

builder.Services.AddCors(p =>
    p.AddPolicy(
        "corsapp",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    )
);

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            ValidIssuer = issuerHost,
        };
    });

builder.Services.AddBusinessServiceExtensions();

builder
    .Services.AddHangfire(configuration =>
        configuration
            ?.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(x => x.UseNpgsqlConnection(dbConnectString))
    )
    .AddHangfireServer(options =>
    {
        options.Queues = HangfireConstants.Queues.FullList;
    });
var useLiveMatch = bool.Parse(config.GetSection("UseLiveMatchService")?.Value ?? "false");

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
    app.UseHangfireDashboard("/api/hangfire");
}
else
{
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseResponseCompression();
app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultMiddleware();
app.MapFsCoreHubs(useLiveMatch);
app.MapControllers();
#pragma warning disable ASP0014
if (bool.Parse(useStaticFiles) is true)
{
    app.UseEndpoints(endpoint =>
    {
        endpoint.MapFallbackToFile("index.html");
    });
#pragma warning restore ASP0014
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
                        MaxAge = TimeSpan.Zero,
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
                        MaxAge = TimeSpan.FromDays(365),
                    };
                }
            },
        };
    });
}

await app.RunAsync();

public static partial class Program { };
