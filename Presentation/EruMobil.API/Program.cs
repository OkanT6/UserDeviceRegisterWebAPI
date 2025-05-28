using EruMobil.Application;
using EruMobil.Mapper;
using EruMobil.Persistence;
using Microsoft.OpenApi.Models;
using EruMobil.Application.Exceptions;
using EruMobil.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor(); // ⬅️ Bunu ekle!
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddCustomMapper();

// MediatR'ı manuel olarak ekleyin (eğer AddApplication'da yoksa)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(EruMobil.Application.Features.Auth.Commands.Register.RegisterCommandHandler).Assembly);
});

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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "EruMobil API",
        Version = "v1",
        Description = "EruMobil API Documentation"
    });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.ConfigureExceptionHandlingMiddleware();

// Önemli sıralama:
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();