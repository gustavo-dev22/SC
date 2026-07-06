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
    public class PostulanteFormacionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostulanteFormacionController(IMediator mediator) => _mediator = mediator;

        [HttpGet("postulante/{idPostulante}")]
        public async Task<IActionResult> Get(int idPostulante)
        {
            var result = await _mediator.Send(new GetFormacionQuery(idPostulante));
            return Ok(BaseResponse<List<PostulanteFormacionDto>>.Ok(result));
        }

        [HttpPost("mantenimiento")]
        public async Task<IActionResult> Mantenimiento([FromBody] MantenimientoFormacionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(BaseResponse<bool>.Ok(result, "Operación procesada con éxito."));
        }

        [HttpPost("subir-sustento")]
        public async Task<IActionResult> SubirSustento([FromForm] int idFormacion, [FromForm] IFormFile archivo)
        {
            // Instanciamos el comando universal pasándole la etiqueta identificadora
            var command = new SubirSustentoCommand(idFormacion, archivo, "FORMACION");
            var resultado = await _mediator.Send(command);

            if (resultado) return Ok(new { success = true, message = "Archivo sustentatorio de formación guardado correctamente." });
            return BadRequest(new { success = false, message = "No se pudo procesar el archivo cargado." });
        }

        [HttpDelete("eliminar-sustento/{idFormacion}")]
        public async Task<IActionResult> EliminarSustento(int idFormacion)
        {
            var resultado = await _mediator.Send(new EliminarSustentoCommand(idFormacion, "FORMACION"));
            if (resultado) return Ok(new { success = true, message = "Sustento de formación eliminado correctamente." });
            return BadRequest(new { success = false, message = "No se pudo eliminar el sustento." });
        }
    }
}
