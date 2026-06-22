using Application.Postulantes.Commands;
using Application.Postulantes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SoporteController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SoporteController(IMediator mediator) => _mediator = mediator;

        [HttpGet("tickets/{idPostulante:int}")]
        public async Task<IActionResult> GetTickets(int idPostulante)
        {
            var resultado = await _mediator.Send(new GetTicketsQuery(idPostulante));
            return Ok(new { success = true, data = resultado });
        }

        [HttpPost("registrar-ticket")]
        public async Task<IActionResult> RegistrarTicket([FromBody] RegistrarTicketCommand command)
        {
            try
            {
                var exito = await _mediator.Send(command);
                if (!exito)
                {
                    // 🚀 DIAGNÓSTICO 1: Si el handler devuelve false sin lanzar excepción, avísanos aquí
                    return BadRequest(new
                    {
                        success = false,
                        message = "El Handler devolvió FALSE. El Stored Procedure no insertó filas (filas afectadas = 0)."
                    });
                }
                return Ok(new { success = true, message = "¡Su consulta/reclamo fue enviado de forma exitosa!" });
            }
            catch (Exception ex)
            {
                // 🚀 DIAGNÓSTICO 2: Si revienta, capturamos el StackTrace completo (Error de SQL, tipos, etc.)
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.InnerException?.Message ?? ex.StackTrace
                });
            }
        }
    }
}
