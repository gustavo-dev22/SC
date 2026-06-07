using Application.Common.Dtos;
using Application.Postulantes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UbigeoController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UbigeoController(IMediator mediator) => _mediator = mediator;

        [HttpGet("departamentos")]
        public async Task<IActionResult> GetDepartamentos()
        {
            var result = await _mediator.Send(new GetDepartamentosQuery());
            return Ok(BaseResponse<List<UbigeoDto>>.Ok(result));
        }

        [HttpGet("provincias/{idDepartamento}")]
        public async Task<IActionResult> GetProvincias(string idDepartamento)
        {
            var result = await _mediator.Send(new GetProvinciasQuery(idDepartamento));
            return Ok(BaseResponse<List<UbigeoDto>>.Ok(result));
        }

        [HttpGet("distritos/{idProvincia}")]
        public async Task<IActionResult> GetDistritos(string idProvincia)
        {
            var result = await _mediator.Send(new GetDistritosQuery(idProvincia));
            return Ok(BaseResponse<List<UbigeoDto>>.Ok(result));
        }
    }
}
