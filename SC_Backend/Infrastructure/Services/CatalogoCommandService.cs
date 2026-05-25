using Application.Catalogos.Commands;
using Application.Common.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CatalogoCommandService : ICatalogoCommandService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public CatalogoCommandService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> ProcesarMantenimientoAsync(CatalogoCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Accion", command.Accion);
            parameters.Add("@IdValor", command.IdValor, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            parameters.Add("@IdTipo", command.IdTipo);
            parameters.Add("@CodigoValor", command.CodigoValor);
            parameters.Add("@Descripcion", command.Descripcion);
            parameters.Add("@Orden", command.Orden);
            parameters.Add("@Activo", command.Activo ? 1 : 0, dbType: DbType.Boolean, direction: ParameterDirection.Input);

            await connection.ExecuteAsync(
                "sp_CatalogoValor_Mantenimiento",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }
    }
}
