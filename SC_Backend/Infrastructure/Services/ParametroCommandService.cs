using Application.Common.Interfaces;
using Application.Parametros.Commands;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ParametroCommandService : IParametroCommandService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ParametroCommandService(IDbConnectionFactory dbFactory) => _dbConnectionFactory = dbFactory;

        public async Task<bool> ProcesarMantenimientoAsync(MantenimientoParametroCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                "sp_ParametroGlobal_Mantenimiento",
                new
                {
                    command.Accion,
                    command.Codigo,
                    command.Nombre,
                    command.Valor,
                    command.Descripcion,
                    command.Categoria
                },
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
    }
}
