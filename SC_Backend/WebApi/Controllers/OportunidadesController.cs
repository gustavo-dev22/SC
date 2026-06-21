using Application.Oportunidades.Commands;
using Application.Oportunidades.Queries;
using Application.Postulantes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OportunidadesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OportunidadesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("buscar-plazas/{idPostulante:int}")]
        public async Task<IActionResult> BuscarPlazas(int idPostulante, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var query = new ListarPlazasVacantesQuery(idPostulante, search, page, size);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("registrar-postulacion")]
        public async Task<IActionResult> RegistrarPostulacion([FromBody] RegistrarPostulacionCommand command)
        {
            try
            {
                var exito = await _mediator.Send(command);
                if (!exito) return BadRequest(new { message = "No se pudo completar el registro de la postulación." });
                return Ok(new { message = "¡Postulación registrada de forma exitosa!" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("mis-postulaciones/{idPostulante}")]
        public async Task<IActionResult> GetMisPostulaciones(int idPostulante)
        {
            try
            {
                var resultado = await _mediator.Send(new GetMisPostulacionesQuery(idPostulante));
                return Ok(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error al recuperar el historial de postulaciones.", error = ex.Message });
            }
        }
    }
}
