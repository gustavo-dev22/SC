using Application.Catalogos.Commands;
using Application.Catalogos.Dtos;
using Application.Catalogos.Queries;
using Application.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogoController(IMediator mediator) => _mediator = mediator;

        [HttpGet("valores/{idTipo}")]
        public async Task<IActionResult> GetValores(int idTipo, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetValoresByTipoQuery(idTipo, pageNumber, pageSize));
            return Ok(BaseResponse<List<CatalogoValorDto>>.Ok(result, "Valores recuperados."));
        }

        [HttpGet("valores-por-codigo/{codigo}")]
        public async Task<IActionResult> GetValoresByCodigo(string codigo)
        {
            var result = await _mediator.Send(new GetValoresByCodigoTipoQuery(codigo));

            return Ok(BaseResponse<List<CatalogoValorDto>>.Ok(result, "Valores del catálogo cargados de forma universal."));
        }

        [HttpPost("mantenimiento")]
        public async Task<IActionResult> Mantenimiento([FromBody] CatalogoCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(BaseResponse<bool>.Ok(result, "Operación ejecutada con éxito."));
            }
            catch (SqlException ex) when (ex.Number == 51000)
            {
                return BadRequest(BaseResponse<bool>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(BaseResponse<bool>.Fail("Ocurrió un error inesperado en el servidor: " + ex.Message));
            }
        }

        [HttpGet("tipos")]
        public async Task<IActionResult> GetTipos()
        {
            var result = await _mediator.Send(new GetCatalogoTiposQuery());
            return Ok(BaseResponse<List<CatalogoTipoDto>>.Ok(result, "Tipos de catálogo recuperados."));
        }

        [HttpPost("tipo-mantenimiento")]
        public async Task<IActionResult> MantenimientoTipo([FromBody] TipoCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(BaseResponse<bool>.Ok(result, $"Catálogo raíz '{command.Nombre}' procesado."));
            }
            catch (SqlException ex) when (ex.Number == 51000)
            {
                return BadRequest(BaseResponse<bool>.Fail(ex.Message));
            }
        }

        [HttpGet("centros-estudios")]
        public async Task<IActionResult> GetCentrosEstudios([FromQuery] string query = "")
        {
            var result = await _mediator.Send(new GetCentrosEstudiosQuery(query));
            return Ok(BaseResponse<List<CentroEstudioDto>>.Ok(result, "Centros de estudio unificados cargados con éxito."));
        }
    }
}
