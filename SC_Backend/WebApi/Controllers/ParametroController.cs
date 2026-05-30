using Application.Common.Dtos;
using Application.Parametros.Commands;
using Application.Parametros.Dtos;
using Application.Parametros.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParametroController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ParametroController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetParametrosQuery());
            return Ok(BaseResponse<List<ParametroGlobalDto>>.Ok(result, "Parámetros globales cargados."));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateParametroCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(BaseResponse<bool>.Ok(result, "Configuración actualizada correctamente."));
        }
    }
}
