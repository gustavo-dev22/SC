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
    public class PostulanteInfoAdicionalController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostulanteInfoAdicionalController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{idPostulante}")]
        public async Task<IActionResult> GetInfoAdicional(int idPostulante)
        {
            var result = await _mediator.Send(new ObtenerInfoAdicionalQuery(idPostulante));
            return Ok(BaseResponse<InfoAdicionalDto>.Ok(result));
        }

        [HttpPost("postulante/guardar")]
        public async Task<IActionResult> GuardarInfoAdicional([FromBody] GuardarInfoAdicionalCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(BaseResponse<bool>.Ok(result, "Información de disponibilidad actualizada correctamente."));
        }
    }
}
