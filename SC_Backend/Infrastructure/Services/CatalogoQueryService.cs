using Application.Catalogos.Dtos;
using Application.Common.Interfaces;
using System.Data;
using Dapper;

namespace Infrastructure.Services
{
    public class CatalogoQueryService : ICatalogoQueryService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public CatalogoQueryService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<List<CatalogoValorDto>> ObtenerValoresByTipoAsync(int idTipo)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            const string storedProcedure = "sp_CatalogoValor_ListarByTipo";
            var parameters = new { IdTipo = idTipo };

            var result = await connection.QueryAsync<CatalogoValorDto>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }
}
