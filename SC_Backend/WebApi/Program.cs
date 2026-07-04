using Application;
using Application.Common.Interfaces;
using Infrastructure;
using Infrastructure.Services;
using QuestPDF.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient("SistemaPublicacionConvocatorias", client =>
{
    // IMPORTANTE: Recuerda poner el "/" al final de la URL base para que Spring Boot ensamble bien las rutas
    client.BaseAddress = new Uri("http://localhost:8080/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsAngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200", "http://localhost:4201", "http://127.0.0.1:4201") // Permite el origen de tu frontend
              .AllowAnyMethod()                                             // Permite POST, GET, OPTIONS, PUT, DELETE
              .AllowAnyHeader();                                          
    });
});

QuestPDF.Settings.License = LicenseType.Community;

var webRootPath = builder.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

builder.Services.AddScoped<IFileStorageService>(provider =>
    new FileStorageService(webRootPath)
);

var app = builder.Build();

// =========================================================================
// CONFIGURACIÓN DEL PIPELINE DE PETICIONES (Middlewares)
// =========================================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Sistema de Convocatorias Core API")
            .WithTheme(ScalarTheme.DeepSpace) // Un tema oscuro moderno
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseCors("CorsAngularPolicy");
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

app.Run();
