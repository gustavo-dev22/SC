using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            // 2. Agrupación limpia de Servicios de Catálogos
            services.AddScoped<ICatalogoQueryService, CatalogoQueryService>();
            services.AddScoped<ICatalogoCommandService, CatalogoCommandService>();

            // 3. Agrupación limpia de Parámetros del Sistema
            services.AddScoped<IParametroQueryService, ParametroQueryService>();
            services.AddScoped<IParametroCommandService, ParametroCommandService>();

            // 4. Agrupación limpia de Postulantes (Datos Personales, Formación y la nueva de Certificaciones)
            services.AddScoped<IPostulanteQueryService, PostulanteQueryService>();
            services.AddScoped<IPostulanteCommandService, PostulanteCommandService>();

            services.AddScoped<IOportunidadesQueryService, OportunidadesQueryService>();
            services.AddScoped<IAdminSoporteService, AdminSoporteService>();
            services.AddScoped<IAdminPostulacionService, AdminPostulacionService>();
            services.AddScoped<IComiteEvaluadorService, ComiteEvaluadorService>();
            services.AddScoped<IFileStorageService>(provider =>
            {
                // Buscamos la ruta base de ejecución del servidor y apuntamos a wwwroot de forma segura
                string baseDir = System.AppContext.BaseDirectory;

                // Fail-safe: retrocedemos dinámicamente si es que el binario corre en subcarpetas /net8.0/ debug
                string webRootPath = System.IO.Path.Combine(baseDir, "wwwroot");

                // Si por temas de compilación no existe a este nivel, buscamos la ruta física real del proyecto
                if (!System.IO.Directory.Exists(webRootPath))
                {
                    webRootPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");
                }

                // Retornamos la instancia resolviendo el parámetro string de forma manual 🎯
                return new Infrastructure.Services.FileStorageService(webRootPath);
            });

            // Retornamos el contenedor para permitir encadenamiento (Fluent API)
            return services;
        }
    }
}
