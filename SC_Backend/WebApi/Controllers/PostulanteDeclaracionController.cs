using Application.Postulantes.Commands;
using Application.Postulantes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostulanteDeclaracionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostulanteDeclaracionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 🚀 ENDPOINT 1: Listar las DDJJ dinámicas del Postulante
        [HttpGet("listar/{idPostulante:int}/{idTipo:int}")]
        public async Task<IActionResult> GetDeclaraciones(int idPostulante, int idTipo)
        {
            var resultado = await _mediator.Send(new ObtenerDeclaracionesQuery(idPostulante, idTipo));
            return Ok(resultado);
        }

        // 🚀 ENDPOINT 2: Guardar las DDJJ marcadas masivamente
        [HttpPost("guardar")]
        public async Task<IActionResult> GuardarDeclaraciones([FromBody] GuardarDeclaracionesCommand command)
        {
            var exito = await _mediator.Send(command);
            if (!exito) return BadRequest(new { message = "Ocurrió un error al registrar las declaraciones juradas." });
            return Ok(new { message = "Declaraciones Juradas registradas correctamente." });
        }
    }
}
