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
    }
}
