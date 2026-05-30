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

        public async Task<List<CatalogoValorDto>> ObtenerValoresByTipoAsync(int idTipo, int pageNumber, int pageSize)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            return (await connection.QueryAsync<CatalogoValorDto>(
                "sp_CatalogoValor_ListarByTipo",
                new { IdTipo = idTipo, PageNumber = pageNumber, PageSize = pageSize },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<CatalogoTipoDto>> ObtenerTiposActivosAsync()
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<CatalogoTipoDto>(
                "sp_CatalogoTipo_ListarActivos",
                commandType: CommandType.StoredProcedure
            )).ToList();
        }
    }
}
