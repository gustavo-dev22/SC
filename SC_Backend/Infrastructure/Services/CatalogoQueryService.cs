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

        public async Task<List<CatalogoValorDto>> ListarValoresByCodigoTipoAsync(string codigoTipo)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var resultado = await connection.QueryAsync<CatalogoValorDto>(
                "sp_CatalogoValor_ListarByCodigoTipo",
                new { CodigoTipo = codigoTipo },
                commandType: CommandType.StoredProcedure
            );

            return resultado.ToList();
        }

        public async Task<List<CentroEstudioDto>> ListarInstitutosPredictivoAsync(string filtro)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var parametros = new { TextoBusqueda = filtro };

            var dbResult = await connection.QueryAsync<DapperPredictivoResult>(
                "sp_Institutos_ListarPredictivo",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return dbResult.Select(i => new CentroEstudioDto
            {
                Nombre = i.Descripcion.ToUpper(),
                TipoProvider = "INSTITUTO"
            }).ToList();
        }

        public async Task<List<CentroEstudioDto>> ListarEntidadesPublicasPredictivoAsync(string filtro)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var parametros = new { TextoBusqueda = filtro };

            var dbResult = await connection.QueryAsync<DapperPredictivoResult>(
                "sp_Entidades_ListarPredictivo",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return dbResult.Select(i => new CentroEstudioDto
            {
                Nombre = i.Descripcion.ToUpper(),
                TipoProvider = "PUBLICO"
            }).ToList();
        }

        private class DapperPredictivoResult { public int Id { get; set; } public string Descripcion { get; set; } = string.Empty; }
    }
}
