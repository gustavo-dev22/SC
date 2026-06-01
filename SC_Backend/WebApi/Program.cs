using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// REGISTRO DE CAPAS (Inyección de Dependencias)
// =========================================================================

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ICatalogoQueryService, CatalogoQueryService>();
builder.Services.AddScoped<ICatalogoCommandService, CatalogoCommandService>();
builder.Services.AddScoped<IParametroQueryService, ParametroQueryService>();
builder.Services.AddScoped<IParametroCommandService, ParametroCommandService>();
builder.Services.AddScoped<IPostulanteCommandService, PostulanteCommandService>();
builder.Services.AddScoped<IPostulanteQueryService, PostulanteQueryService>();
builder.Services.AddScoped<IPostulanteQueryService, PostulanteQueryService>();
builder.Services.AddScoped<IPostulanteCommandService, PostulanteCommandService>();

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IDbConnectionFactory).Assembly));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsAngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200") // Permite el origen de tu frontend
              .AllowAnyMethod()                                             // Permite POST, GET, OPTIONS, PUT, DELETE
              .AllowAnyHeader();                                          
    });
});

var app = builder.Build();

// =========================================================================
// CONFIGURACIÓN DEL PIPELINE DE PETICIONES (Middlewares)
// =========================================================================

if (app.Environment.IsDevelopment())
{
    // Genera el endpoint /openapi/v1.json automáticamente
    app.MapOpenApi();

    // Renderiza la interfaz de Scalar en la ruta /scalar
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Sistema de Convocatorias Core API")
            .WithTheme(ScalarTheme.DeepSpace) // Un tema oscuro moderno
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient); // Generador de código por defecto para C#
    });
}

app.UseHttpsRedirection();
app.UseCors("CorsAngularPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
