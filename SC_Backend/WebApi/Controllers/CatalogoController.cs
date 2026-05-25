using Application.Catalogos.Commands;
using Application.Catalogos.Dtos;
using Application.Catalogos.Queries;
using Application.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogoController(IMediator mediator) => _mediator = mediator;

        [HttpGet("valores/{idTipo}")]
        public async Task<IActionResult> GetValores(int idTipo)
        {
            var result = await _mediator.Send(new GetValoresByTipoQuery(idTipo));
            return Ok(BaseResponse<List<CatalogoValorDto>>.Ok(result, "Valores del catálogo recuperados."));
        }

        [HttpPost("mantenimiento")]
        public async Task<IActionResult> Mantenimiento([FromBody] CatalogoCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(BaseResponse<bool>.Ok(result, $"Operación '{command.Accion}' ejecutada correctamente."));
        }
    }
}
