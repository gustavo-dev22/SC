using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Admin.Dtos;

namespace Application.Common.Interfaces
{
    public interface IAdminSoporteService
    {
        Task<List<AdminTicketBandejaDto>> ObtenerBandejaTicketsAsync(int? idEstado, string? busqueda);
        Task<TicketResolucionResultadoDto?> ResolverTicketAsync(int idTicket, string respuesta, int idEstado, string nombreUsuarioAdmin);
        Task<TicketResolucionResultadoDto?> RecepcionarTicketAsync(int idTicket);
        Task<TicketResolucionResultadoDto?> CambiarEstadoTicketAsync(int idTicket, string? respuesta, int idEstado);
        Task<List<AuditoriaLogDto>> ObtenerLogsAuditoriaAsync(string? tabla, string? operacion, DateTime? fechaInicio, DateTime? fechaFin);
        Task<DashboardSummaryDto> ObtenerResumenDashboardAsync();
    }
}
