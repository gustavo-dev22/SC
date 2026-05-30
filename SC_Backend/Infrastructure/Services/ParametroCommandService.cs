using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Dapper;

namespace Infrastructure.Services
{
    public class ParametroCommandService : IParametroCommandService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public ParametroCommandService(IDbConnectionFactory dbFactory) => _dbConnectionFactory = dbFactory;

        public async Task<bool> ActualizarParametroAsync(string codigo, string valor)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                "sp_ParametroGlobal_Actualizar",
                new { Codigo = codigo, Valor = valor },
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
    }
}
