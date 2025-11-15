using System.IO.Compression;
using System.Text;
using FlightWatch.Api.Configuration;
using FlightWatch.Api.Extensions;
using FlightWatch.Api.HealthChecks;
using FlightWatch.Api.Hubs;
using FlightWatch.Api.Services;
using FlightWatch.Application.Configuration;
using FlightWatch.Application.Interfaces;
using FlightWatch.Infrastructure.BackgroundServices;
using FlightWatch.Infrastructure.Configuration;
using FlightWatch.Infrastructure.Extensions;
using FlightWatch.Infrastructure.Data;
using FlightWatch.Infrastructure.EventStore;
using FlightWatch.Infrastructure.Repositories;
using FlightWatch.Infrastructure.Security;
using FlightWatch.Infrastructure.Services;
using MongoDB.Driver;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting FlightWatch API");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "FlightWatch.Api")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .Enrich.WithMachineName()
        .Enrich.WithThreadId());

    builder.Services.AddTelemetry(builder.Configuration);

    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(FlightWatch.Application.Features.Auth.Commands.Login.LoginCommand).Assembly);
        cfg.AddOpenBehavior(typeof(FlightWatch.Application.Behaviors.LoggingBehavior<,>));
        cfg.AddOpenBehavior(typeof(FlightWatch.Application.Behaviors.ValidationBehavior<,>));
        cfg.AddOpenBehavior(typeof(FlightWatch.Application.Behaviors.PerformanceBehavior<,>));
    });

    builder.Services.AddAutoMapper(typeof(FlightWatch.Application.Mappings.AuthMappingProfile).Assembly);

    builder.Services.Configure<OpenSkySettings>(
        builder.Configuration.GetSection("OpenSky"));
        
    var openSkySettings = builder.Configuration.GetSection("OpenSky").Get<OpenSkySettings>();
    Log.Information("OpenSky Configuration - BaseUrl: {BaseUrl}", openSkySettings?.BaseUrl ?? "NOT SET");
    Log.Information("OpenSky Configuration - ClientId: {ClientId}", 
        string.IsNullOrEmpty(openSkySettings?.ClientId) ? "NOT SET" : $"{openSkySettings.ClientId[..Math.Min(4, openSkySettings.ClientId.Length)]}***");
    Log.Information("OpenSky Configuration - ClientSecret: {ClientSecret}", 
        string.IsNullOrEmpty(openSkySettings?.ClientSecret) ? "NOT SET" : "***SET***");

    builder.Services.Configure<JwtSettings>(
        builder.Configuration.GetSection("JwtSettings"));

    builder.Services.Configure<MongoDbSettings>(
        builder.Configuration.GetSection("MongoDB"));

    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    var secretKey = Encoding.UTF8.GetBytes(jwtSettings!.SecretKey);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    var mongoDbSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>();
    if (mongoDbSettings == null || string.IsNullOrEmpty(mongoDbSettings.ConnectionString))
    {
        throw new InvalidOperationException("MongoDB configuration is required. Please set MongoDB__ConnectionString and MongoDB__DatabaseName in configuration.");
    }

    var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
    builder.Services.AddSingleton<IMongoClient>(mongoClient);
    builder.Services.AddScoped<MongoDbContext>(sp =>
    {
        var client = sp.GetRequiredService<IMongoClient>();
        return new MongoDbContext(client, mongoDbSettings.DatabaseName);
    });

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    builder.Services.AddScoped<IFlightSubscriptionRepository, FlightSubscriptionRepository>();
    builder.Services.AddScoped<IEventStore, EventStore>();

    Log.Information("MongoDB configured with database: {DatabaseName}", mongoDbSettings.DatabaseName);
    
    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IFlightNotificationService, FlightNotificationService>();
    builder.Services.AddScoped<IFlightMonitoringService, FlightMonitoringService>();

    builder.Services.AddSingleton<FlightWatch.Infrastructure.ExternalServices.OpenSky.OpenSkyAuthClient>();
    
    builder.Services.AddHttpClient<IOpenSkyClient, FlightWatch.Infrastructure.ExternalServices.OpenSky.OpenSkyClient>()
        .AddPolicyHandler(ResiliencePolicies.GetRetryPolicy())
        .AddPolicyHandler(ResiliencePolicies.GetTimeoutPolicy())
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        .ConfigureHttpClient(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
        });

    builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = builder.Environment.IsDevelopment();
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    });

    builder.Services.AddHostedService<FlightUpdateBackgroundService>();

    builder.Services.AddEventSourcing(builder.Configuration);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSignalR", policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                policy.SetIsOriginAllowed(_ => true)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            }
            else
            {
                policy.WithOrigins("https://yourdomain.com")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            }
        });
    });

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "FlightWatch API",
            Version = "v1",
            Description = "API profissional para rastreamento de voos em tempo real usando AviationStack"
        });
    });

    builder.Services.AddProblemDetails();
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[]
            {
                "application/json",
                "application/javascript",
                "text/html",
                "text/css",
                "text/plain",
                "application/xml",
                "text/xml"
            });
    });

    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest;
    });

    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest;
    });

    var rabbitMqSettings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>() ?? new RabbitMqSettings();
    var rabbitConnectionString = $"amqp://{rabbitMqSettings.Username}:{rabbitMqSettings.Password}@{rabbitMqSettings.Host}:{rabbitMqSettings.Port}";

    builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy("API is running"), tags: ["api"])
        .AddCheck<OpenSkyHealthCheck>("opensky-api", tags: ["external", "api"])
        .AddCheck<SignalRHealthCheck>("signalr-hub", tags: ["signalr"])
        .AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: ["messaging", "rabbitmq"]);

    builder.Services.AddHealthChecksUI(setup =>
    {
        setup.SetEvaluationTimeInSeconds(30);
        setup.MaximumHistoryEntriesPerEndpoint(50);
        setup.AddHealthCheckEndpoint("FlightWatch API", "/health");
    })
    .AddInMemoryStorage();

    var app = builder.Build();

    app.UseResponseCompression();

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value!);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
        };
    });

    app.UseCorrelationId();

    app.UseGlobalExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "FlightWatch API V1");
            c.RoutePrefix = "api/swagger";
        });
    }

    app.UseHttpsRedirection();

    app.UseCors("AllowSignalR");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    
    app.MapHub<FlightHub>("/hubs/flights");

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        ResultStatusCodes =
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Degraded] = StatusCodes.Status200OK,
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        }
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("api"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecksUI(options =>
    {
        options.UIPath = "/health-ui";
        options.ApiPath = "/health-ui-api";
        options.UseRelativeApiPath = false;
        options.UseRelativeResourcesPath = false;
    });

    Log.Information("FlightWatch API started successfully");
    Log.Information("Health Check UI available at: /health-ui");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "FlightWatch API failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
