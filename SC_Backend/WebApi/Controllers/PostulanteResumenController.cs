using Application.Common.Dtos;
using Application.Postulantes.Dtos;
using Application.Postulantes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostulanteResumenController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostulanteResumenController(IMediator mediator) => _mediator = mediator;

        [HttpGet("avance/{idPostulante}")]
        public async Task<IActionResult> GetAvance(int idPostulante)
        {
            var result = await _mediator.Send(new GetAvanceCurriculumQuery(idPostulante));
            return Ok(BaseResponse<AvanceCurriculumDto>.Ok(result));
        }
    }
}
