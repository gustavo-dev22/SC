using Application.Common.Dtos;
using Application.Parametros.Commands;
using Application.Parametros.Dtos;
using Application.Parametros.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParametroController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ParametroController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? codigo)
        {
            var result = await _mediator.Send(new GetParametrosQuery(codigo));
            return Ok(BaseResponse<List<ParametroGlobalDto>>.Ok(result, "Operación procesada de manera conforme."));
        }

        [HttpPost("mantenimiento")]
        public async Task<IActionResult> Mantenimiento([FromBody] MantenimientoParametroCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(BaseResponse<bool>.Ok(result, "Operación procesada con éxito."));
            }
            catch (SqlException ex) when (ex.Number == 51000)
            {
                return BadRequest(BaseResponse<bool>.Fail(ex.Message));
            }
        }

        [HttpGet("publico/mantenimiento")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidarMantenimiento()
        {
            // Despachamos la Query directa a través del Mediator sin romper la arquitectura
            var enMantenimiento = await _mediator.Send(new GetMantenimientoPortalQuery());

            return Ok(new { success = true, enMantenimiento });
        }
    }
}
