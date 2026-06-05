using Application;
using Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

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

app.Run();
