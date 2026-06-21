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

            // Retornamos el contenedor para permitir encadenamiento (Fluent API)
            return services;
        }
    }
}
