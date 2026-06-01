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
    public class PostulantePerfilController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostulantePerfilController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetPerfilPostulanteQuery(id));
            return Ok(BaseResponse<PerfilPostulanteDto>.Ok(result!, "Perfil del postulante cargado."));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ActualizarPerfilCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(BaseResponse<bool>.Ok(result, "Datos personales actualizados correctamente."));
        }
    }
}
