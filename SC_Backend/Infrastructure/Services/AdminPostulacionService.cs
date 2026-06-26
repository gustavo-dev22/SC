using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Admin.Dtos;
using Application.Common.Interfaces;
using Dapper;

namespace Infrastructure.Services
{
    public class AdminPostulacionService : IAdminPostulacionService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public AdminPostulacionService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<List<TrazabilidadExpedienteDto>> ObtenerTrazabilidadPorExpedienteAsync(string codigoExpediente)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var result = await connection.QueryAsync<TrazabilidadExpedienteDto>(
                "sp_Postulacion_ObtenerTrazabilidad",
                new { CodigoExpediente = codigoExpediente },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }
}
