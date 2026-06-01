using Application.Common.Interfaces;
using Application.Postulantes.Commands;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PostulanteCommandService : IPostulanteCommandService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public PostulanteCommandService(IDbConnectionFactory dbFactory) => _dbConnectionFactory = dbFactory;

        public async Task<bool> RegistrarPostulanteAsync(RegistrarPostulanteCommand command, string computedHash)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@NumDocumento", command.NumDocumento);
            parameters.Add("@Nombres", command.Nombres);
            parameters.Add("@ApellidoPaterno", command.ApellidoPaterno);
            parameters.Add("@ApellidoMaterno", command.ApellidoMaterno);
            parameters.Add("@Correo", command.Correo);
            parameters.Add("@PasswordHash", computedHash);
            parameters.Add("@IdPostulante", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("sp_Postulante_Registrar", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<bool> ActualizarPerfilAsync(ActualizarPerfilCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                "sp_Postulante_ActualizarPerfil",
                new
                {
                    command.IdPostulante,
                    command.Telefono,
                    command.FechaNacimiento,
                    command.IdSexoCat,
                    command.Direccion
                },
                commandType: CommandType.StoredProcedure
            );
            return true;
        }

        public async Task<bool> MantenimientoFormacionAsync(MantenimientoFormacionCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            await connection.ExecuteAsync("sp_PostulanteFormacion_Mantenimiento", command, commandType: CommandType.StoredProcedure);
            return true;
        }
    }
}
