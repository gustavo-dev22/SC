using Application.Admin.Commands;
using Application.Admin.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminSoporteController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminSoporteController(IMediator mediator) => _mediator = mediator;

        [HttpGet("bandeja")]
        public async Task<IActionResult> GetBandeja([FromQuery] int? idEstado, [FromQuery] string? busqueda)
        {
            var data = await _mediator.Send(new GetBandejaTicketsQuery(idEstado, busqueda));
            return Ok(new { success = true, data });
        }

        [HttpPost("atender")]
        public async Task<IActionResult> Atender([FromBody] AtenderTicketCommand command)
        {
            var exito = await _mediator.Send(command);
            if (!exito) return BadRequest(new { message = "No se pudo actualizar el ticket." });
            return Ok(new { success = true, message = "El ticket fue resuelto y notificado de forma conforme." });
        }

        [HttpGet("logs-auditoria")]
        public async Task<IActionResult> GetLogs([FromQuery] string? tabla, [FromQuery] string? operacion, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            var data = await _mediator.Send(new GetLogsAuditoriaQuery(tabla, operacion, fechaInicio, fechaFin));
            return Ok(new { success = true, data });
        }

        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var data = await _mediator.Send(new GetDashboardSummaryQuery());
            return Ok(new { success = true, data });
        }
    }
}
