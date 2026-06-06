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
    public class PostulanteOtrosRequisitosController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostulanteOtrosRequisitosController(IMediator mediator) => _mediator = mediator;

        [HttpGet("postulante/{idPostulante}")]
        public async Task<IActionResult> Get(int idPostulante)
        {
            var result = await _mediator.Send(new GetRequisitosEspecialesQuery(idPostulante));
            return Ok(BaseResponse<List<PostulanteRequisitoEspecialDto>>.Ok(result));
        }

        [HttpPost("mantenimiento")]
        public async Task<IActionResult> Mantenimiento([FromBody] MantenimientoRequisitoEspecialCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(BaseResponse<bool>.Ok(result, "Requisito especial procesado correctamente."));
        }
    }
}
