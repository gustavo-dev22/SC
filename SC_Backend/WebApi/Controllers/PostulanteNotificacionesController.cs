using Application.Postulantes.Commands;
using Application.Postulantes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostulanteNotificacionesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostulanteNotificacionesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{idPostulante:int}")]
        public async Task<IActionResult> GetNotificaciones(int idPostulante)
        {
            try
            {
                var resultado = await _mediator.Send(new GetNotificacionesQuery(idPostulante));
                return Ok(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("marcar-leida")]
        public async Task<IActionResult> MarcarLeida([FromBody] MarcarNotificacionLeidaCommand command)
        {
            try
            {
                var exito = await _mediator.Send(command);
                if (!exito)
                    return BadRequest(new { success = false, message = "No se pudo actualizar el estado de la notificación." });

                return Ok(new { success = true, message = "Notificación marcada como leída de manera conforme." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
