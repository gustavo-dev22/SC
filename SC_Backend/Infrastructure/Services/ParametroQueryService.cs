using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Parametros.Dtos;
using Dapper;

namespace Infrastructure.Services
{
    public class ParametroQueryService : IParametroQueryService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public ParametroQueryService(IDbConnectionFactory dbFactory) => _dbConnectionFactory = dbFactory;

        public async Task<List<ParametroGlobalDto>> ObtenerParametrosAsync(string? codigo)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<ParametroGlobalDto>(
                "sp_ParametroGlobal_Listar",
                new { Codigo = codigo },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<bool> ObtenerEstadoMantenimientoAsync()
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            const string sql = "SELECT valor FROM sc_parametro_global WHERE codigo = 'FLAG_MANTENIMIENTO_PORTAL' AND activo = 1";

            var valorStr = await connection.QueryFirstOrDefaultAsync<string>(sql);

            return valorStr == "1";
        }
    }
}
