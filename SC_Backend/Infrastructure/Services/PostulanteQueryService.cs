using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Postulantes.Dtos;
using Dapper;

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

        public async Task<List<PostulanteIdiomaDto>> ListarIdiomasAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteIdiomaDto>(
                "sp_PostulanteIdioma_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<PostulanteOfimaticaDto>> ListarOfimaticaAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteOfimaticaDto>(
                "sp_PostulanteOfimatica_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<PostulanteRequisitoEspecialDto>> ListarRequisitosEspecialesAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteRequisitoEspecialDto>(
                "sp_PostulanteRequisitoEspecial_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<AvanceCurriculumDto> ObtenerAvanceCurriculumAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AvanceCurriculumDto>(
                "sp_Postulante_ObtenerAvanceCurriculum",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            ) ?? new AvanceCurriculumDto();
        }

        public async Task<List<UbigeoDto>> ObtenerDepartamentosAsync()
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            const string sql = "SELECT id_departamento AS Id, nombre AS Nombre FROM sc_ubigeo_departamento WHERE activo = 1 ORDER BY nombre";
            return (await connection.QueryAsync<UbigeoDto>(sql)).ToList();
        }

        public async Task<List<UbigeoDto>> ObtenerProvinciasAsync(string idDepartamento)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            const string sql = "SELECT id_provincia AS Id, nombre AS Nombre FROM sc_ubigeo_provincia WHERE id_departamento = @IdDep AND activo = 1 ORDER BY nombre";
            return (await connection.QueryAsync<UbigeoDto>(sql, new { IdDep = idDepartamento })).ToList();
        }

        public async Task<List<UbigeoDto>> ObtenerDistritosAsync(string idProvincia)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            const string sql = "SELECT id_distrito AS Id, nombre AS Nombre FROM sc_ubigeo_distrito WHERE id_provincia = @IdProv AND activo = 1 ORDER BY nombre";
            return (await connection.QueryAsync<UbigeoDto>(sql, new { IdProv = idProvincia })).ToList();
        }

        public async Task<InfoAdicionalDto?> ObtenerInfoAdicionalAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "sp_Postulante_ObtenerInfoAdicional",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            );

            var info = await multi.ReadFirstOrDefaultAsync<dynamic>();
            if (info == null) return new InfoAdicionalDto(idPostulante, false, new List<string>());

            var deptsIds = (await multi.ReadAsync<string>()).ToList();

            return new InfoAdicionalDto(
                (int)info.IdPostulante,
                (bool)info.DisponibilidadInterior,
                deptsIds
            );
        }
    }
}
