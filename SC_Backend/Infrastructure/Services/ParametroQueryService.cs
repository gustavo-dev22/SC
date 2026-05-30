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

        public async Task<List<ParametroGlobalDto>> ObtenerParametrosAsync()
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<ParametroGlobalDto>(
                "sp_ParametroGlobal_Listar",
                commandType: CommandType.StoredProcedure
            )).ToList();
        }
    }
}
