
using Serilog;
using Serilog.Events;
using EruMobil.Application;
using EruMobil.Mapper;
using EruMobil.Persistence;
using Microsoft.OpenApi.Models;
using EruMobil.Application.Exceptions;
using EruMobil.Infrastructure;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog Konfigürasyonu (builder'dan sonra)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.Seq(
        serverUrl: "http://localhost:5341",
        apiKey: builder.Configuration["Seq:ApiKey"])
    .CreateLogger();

try
{
    // 2. Logging Provider'ını Serilog olarak ayarla
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddCustomMapper();



    var env = builder.Environment;

    builder.Configuration
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(System.Net.IPAddress.Any, 5000); // HTTP
        options.Listen(System.Net.IPAddress.Any, 5001, listenOptions =>
        {
            listenOptions.UseHttps();
        });
    });

    // 3. Rate Limiting Loglama Entegrasyonu
    builder.Services.AddRateLimiter(options =>
    {
        options.OnRejected = (context, cancellationToken) =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Rate limit exceeded for {IP}. Path: {Path}",
                context.HttpContext.Connection.RemoteIpAddress,
                context.HttpContext.Request.Path);
            return ValueTask.CompletedTask;
        };

        options.AddPolicy("IPPolicy", context =>
        {
            var ip = context.Connection.RemoteIpAddress ?? System.Net.IPAddress.None;
            var ipString = ip.ToString();

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: ipString,
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromSeconds(50), 
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 2
                });
        });
    });

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "EruMobil API",
            Version = "v1",
            Description = "EruMobil API Documentation"
        });
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.WithOrigins("http://10.130.0.48:5000", "https://10.130.0.48:5001")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.ConfigureExceptionHandlingMiddleware();

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama başlatılamadı");
}
finally
{
    Log.CloseAndFlush();
}