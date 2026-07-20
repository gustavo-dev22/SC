using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Oportunidades.Dtos;
using Dapper;

namespace Infrastructure.Services
{
    public class OportunidadesQueryService : IOportunidadesQueryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public OportunidadesQueryService(IHttpClientFactory httpClientFactory, IDbConnectionFactory dbConnectionFactory)
        {
            _httpClientFactory = httpClientFactory;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<PaginatedSpcResponseDto<PlazaVacanteDto>> ObtenerPlazasDisponiblesAsync(int idPostulante, string? search, int page, int size)
        {
            var resultPaginado = new PaginatedSpcResponseDto<PlazaVacanteDto>();
            var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");

            int javaPage = page - 1;
            if (javaPage < 0) javaPage = 0;

            string url = $"convocatorias/paginado?page={javaPage}&size={size}&sort=fechaCreacion,desc";
            if (!string.IsNullOrEmpty(search))
            {
                url += $"&search={Uri.EscapeDataString(search)}";
            }

            var spcResponse = await client.GetFromJsonAsync<PaginatedSpcResponseDto<PlazaJavaDto>>(url);

            if (spcResponse == null || spcResponse.Content == null || !spcResponse.Content.Any())
                return resultPaginado;

            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            const string sqlLocal = "SELECT id_plaza FROM sc_postulacion WHERE id_postulante = @IdPostulante AND activo = 1";
            var misPostulacionesIds = (await connection.QueryAsync<int>(sqlLocal, new { IdPostulante = idPostulante })).ToList();

            var listaPlazasNormalizadas = spcResponse.Content.Select(j => new PlazaVacanteDto
            {
                IdPlaza = j.IdConvocatoria, 
                CodigoConvocatoria = j.CodigoConvocatoria,
                NombrePuesto = j.Cargo?.NombreCargo ?? "Puesto No Especificado",
                UnidadOrganica = (string.IsNullOrEmpty(j.NombreOficina) || j.NombreOficina == "N/A")
                        ? (j.Sede?.NombreSede ?? "Oficina Institucional")
                        : j.NombreOficina,
                Remuneracion = j.Remuneracion ?? 0.00m,
                //}
                //FechaFin = j.FechaFin ?? DateTime.Now.AddDays(5),
                FechaFin = j.FechaFinPostulacion ?? DateTime.Now.AddDays(2),
                YaPostulo = misPostulacionesIds.Contains(j.IdConvocatoria)
            }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                string criterio = search.ToLower().Trim();
                listaPlazasNormalizadas = listaPlazasNormalizadas
                    .Where(p => p.NombrePuesto.ToLower().Contains(criterio) ||
                                p.UnidadOrganica.ToLower().Contains(criterio))
                    .ToList();

                resultPaginado.TotalElements = listaPlazasNormalizadas.Count;
            }
            else
            {
                resultPaginado.TotalElements = spcResponse.TotalElements;
            }

            resultPaginado.Content = listaPlazasNormalizadas;
            resultPaginado.TotalPages = spcResponse.TotalPages;
            resultPaginado.Size = spcResponse.Size;
            resultPaginado.Number = page;

            return resultPaginado;
        }

        public async Task<List<MisPostulacionesDto>> ObtenerMisPostulacionesAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var misPostulaciones = (await connection.QueryAsync<MisPostulacionesDto>(
                "sp_Postulacion_ListarPorPostulante",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();

            if (!misPostulaciones.Any()) return misPostulaciones;

            // Consumo inter-sistemas vía HTTP para homogeneizar con Spring Boot
            var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");

            foreach (var postu in misPostulaciones)
            {
                try
                {
                    var plazaJava = await client.GetFromJsonAsync<PlazaJavaDto>($"convocatorias/{postu.IdPlaza}");
                    if (plazaJava != null)
                    {
                        postu.CodigoConvocatoria = plazaJava.CodigoConvocatoria;
                        postu.NombrePuesto = plazaJava.Cargo?.NombreCargo ?? "Puesto No Especificado";
                        postu.UnidadOrganica = plazaJava.NombreOficina ?? "Oficina Institucional";
                        postu.Remuneracion = plazaJava.Remuneracion ?? 0.00m;
                    }
                }
                catch
                {
                    postu.NombrePuesto = "Información del puesto temporalmente no disponible";
                }
            }

            return misPostulaciones;
        }

        public async Task<MisPostulacionesDto?> ObtenerPostulacionPorIdAsync(int idPostulacion)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            const string sql = @"
        SELECT 
            p.id_postulacion AS IdPostulacion,
            p.id_plaza AS IdPlaza,
            p.id_postulante AS IdPostulante,
            p.codigo_postulacion_unid AS CodigoPostulacion,
            p.fecha_postulacion AS FechaPostulacion,
            v.descripcion AS EstadoDescripcion
        FROM sc_postulacion p
        INNER JOIN sc_catalogo_valor v ON p.id_estado_postulacion_cat = v.id_valor
        WHERE p.id_postulacion = @IdPostulacion AND v.id_tipo = 5;";

            var datos = await connection.QueryFirstOrDefaultAsync<MisPostulacionesDto>(sql, new { IdPostulacion = idPostulacion });

            if (datos == null) return null;

            // Consumo rápido a Spring Boot para traer los textos de la plaza
            var client = _httpClientFactory.CreateClient("SistemaPublicacionConvocatorias");
            try
            {
                var plazaJava = await client.GetFromJsonAsync<PlazaJavaDto>($"convocatorias/{datos.IdPlaza}");
                if (plazaJava != null)
                {
                    datos.CodigoConvocatoria = plazaJava.CodigoConvocatoria;
                    datos.NombrePuesto = plazaJava.Cargo?.NombreCargo ?? "Puesto No Especificado";
                    datos.UnidadOrganica = plazaJava.NombreOficina ?? "Oficina Institucional";
                    datos.Remuneracion = plazaJava.Remuneracion ?? 0.00m;
                }
            }
            catch
            {
                datos.NombrePuesto = "Información del puesto temporalmente no disponible";
            }

            return datos;
        }
    }
}
