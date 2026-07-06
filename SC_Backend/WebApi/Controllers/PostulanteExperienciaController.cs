using Application.Common.Dtos;
using Application.Postulantes.Commands;
using Application.Postulantes.Dtos;
using Application.Postulantes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostulanteExperienciaController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostulanteExperienciaController(IMediator mediator) => _mediator = mediator;

        [HttpGet("postulante/{idPostulante}")]
        public async Task<IActionResult> Get(int idPostulante)
        {
            var result = await _mediator.Send(new GetExperienciaLaboralQuery(idPostulante));
            return Ok(BaseResponse<List<PostulanteExperienciaDto>>.Ok(result));
        }

        [HttpPost("mantenimiento")]
        public async Task<IActionResult> Mantenimiento([FromBody] MantenimientoExperienciaCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(BaseResponse<bool>.Ok(result, "Registro laboral procesado con éxito."));
        }

        [HttpPost("subir-sustento")]
        public async Task<IActionResult> SubirSustento([FromForm] int idExperiencia, [FromForm] IFormFile archivo)
        {
            // 🚀 Reutiliza el comando universal indicando la sección 'EXPERIENCIA'
            var command = new SubirSustentoCommand(idExperiencia, archivo, "EXPERIENCIA");
            var resultado = await _mediator.Send(command);

            if (resultado) return Ok(new { success = true, message = "Constancia de trabajo guardada correctamente." });
            return BadRequest(new { success = false, message = "No se pudo procesar el archivo de experiencia." });
        }

        [HttpDelete("eliminar-sustento/{idExperiencia}")]
        public async Task<IActionResult> EliminarSustento(int idExperiencia)
        {
            // 🚀 Reutiliza el comando de eliminación universal
            var resultado = await _mediator.Send(new EliminarSustentoCommand(idExperiencia, "EXPERIENCIA"));
            if (resultado) return Ok(new { success = true, message = "Sustento de experiencia removido correctamente." });
            return BadRequest(new { success = false, message = "No se pudo eliminar el archivo." });
        }
    }
}
