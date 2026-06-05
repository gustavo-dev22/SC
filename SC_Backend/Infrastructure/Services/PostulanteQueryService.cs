using Application.Common.Interfaces;
using Application.Postulantes.Dtos;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PostulanteQueryService : IPostulanteQueryService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public PostulanteQueryService(IDbConnectionFactory dbFactory) => _dbConnectionFactory = dbFactory;

        public async Task<dynamic> ObtenerByDocumentoAsync(string numDocumento)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Postulante_ObtenerByDocumento",
                new { NumDocumento = numDocumento },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<PerfilPostulanteDto?> ObtenerPerfilByIdAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PerfilPostulanteDto>(
                "sp_Postulante_ObtenerPerfil",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<List<PostulanteFormacionDto>> ListarFormacionAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteFormacionDto>(
                "sp_PostulanteFormacion_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<PostulanteCertificacionDto>> ListarCertificacionesAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteCertificacionDto>(
                "sp_PostulanteCertificacion_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<PostulanteExperienciaDto>> ListarExperienciaAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteExperienciaDto>(
                "sp_PostulanteExperiencia_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<bool> ExisteSuperposicionLaboralAsync(int idPostulante, int idExperiencia, DateTime fechaInicio, DateTime? fechaFin)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var parametros = new
            {
                IdPostulante = idPostulante,
                IdExperiencia = idExperiencia,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin // Pasamos el null nativo, el SP se encarga del fallback
            };

            int coincidencias = await connection.ExecuteScalarAsync<int>(
                "sp_PostulanteExperiencia_ValidarSuperposicion",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return coincidencias > 0;
        }

        public async Task<List<PostulanteColegiaturaDto>> ListarColegiaturasAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteColegiaturaDto>(
                "sp_PostulanteColegiatura_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }
    }
}
