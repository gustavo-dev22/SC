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
    }
}
