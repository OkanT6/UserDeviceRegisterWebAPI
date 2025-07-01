
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

// Serilog Konfigürasyonu (builder'dan sonra)
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
    // Logging Provider'ını Serilog olarak ayarla
    builder.Host.UseSerilog();

    // Add services to the container. (Servisler)
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    // Katmanlardaki Servis Bağımlılıklarının eklenmesi
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddCustomMapper();


    //Ortam Belirleme
    var env = builder.Environment;

    builder.Configuration
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);


    // Kestrel Sunucusu Konfigürasyonu Dinlenecek Port Numarasının belirlenmesi
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(System.Net.IPAddress.Any, 5000); // HTTP
        options.Listen(System.Net.IPAddress.Any, 5001, listenOptions =>
        {
            listenOptions.UseHttps();
        });
    });

    // Rate Limiting Loglama Entegrasyonu
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

    // Swagger Konfigürasyonu
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "EruMobil API",
            Version = "v1",
            Description = "EruMobil API Documentation"
        });
    });

    // CORS Konfigürasyonu
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.WithOrigins("http://10.102.149.147:5000", "https://10.102.149.147:5001") // Bu ip sunucumuzun (web api'mizin) bulunduğu ip'dir
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });


    var app = builder.Build();

    // Eğer ortam geliştirme ise Swagger'ı etkinleştir
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Global Exception Handling Middleware
    app.ConfigureExceptionHandlingMiddleware();

    app.UseHttpsRedirection();
    app.UseRouting();
    // RateLimiting Middleware'inin eklenmesi
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