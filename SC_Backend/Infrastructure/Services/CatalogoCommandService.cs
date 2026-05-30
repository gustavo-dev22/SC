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
            parameters.Add("@Activo", command.Activo ? 1 : 0, dbType: DbType.Int32, direction: ParameterDirection.Input);

            await connection.ExecuteAsync(
                "sp_CatalogoValor_Mantenimiento",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }

        public async Task<bool> ProcesarMantenimientoTipoAsync(TipoCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Accion", command.Accion);
            parameters.Add("@IdTipo", command.IdTipo, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            parameters.Add("@Codigo", command.Codigo);
            parameters.Add("@Nombre", command.Nombre);
            parameters.Add("@Activo", command.Activo ? 1 : 0, dbType: DbType.Int32);

            await connection.ExecuteAsync(
                "sp_CatalogoTipo_Mantenimiento",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
    }
}
