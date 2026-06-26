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
    public class AdminSoporteService : IAdminSoporteService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public AdminSoporteService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<List<AdminTicketBandejaDto>> ObtenerBandejaTicketsAsync(int? idEstado, string? busqueda)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var result = await connection.QueryAsync<AdminTicketBandejaDto>(
                "sp_SoporteTicket_ListarBandeja",
                new { IdEstadoTicketCat = idEstado, Busqueda = busqueda },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<TicketResolucionResultadoDto?> ResolverTicketAsync(int idTicket, string respuesta, int idEstado, string nombreUsuarioAdmin)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<TicketResolucionResultadoDto>(
                "sp_SoporteTicket_Resolver",
                new
                {
                    IdTicket = idTicket,
                    RespuestaSoporte = respuesta,
                    IdEstadoTicketCat = idEstado
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<TicketResolucionResultadoDto?> RecepcionarTicketAsync(int idTicket)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<TicketResolucionResultadoDto>(
                "sp_SoporteTicket_Recepcionar",
                new { IdTicket = idTicket },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<TicketResolucionResultadoDto?> CambiarEstadoTicketAsync(int idTicket, string? respuesta, int idEstado)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<TicketResolucionResultadoDto>(
                "sp_SoporteTicket_CambiarEstado",
                new
                {
                    IdTicket = idTicket,
                    RespuestaSoporte = respuesta,
                    IdEstadoTicketCat = idEstado
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<List<AuditoriaLogDto>> ObtenerLogsAuditoriaAsync(string? tabla, string? operacion, DateTime? fechaInicio, DateTime? fechaFin)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var result = await connection.QueryAsync<AuditoriaLogDto>(
                "sp_Auditoria_ListarLogs",
                new
                {
                    Tabla = tabla,
                    Operacion = operacion,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }
}
