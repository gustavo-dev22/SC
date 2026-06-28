using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComiteEvaluadorController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ComiteEvaluadorController(IMediator mediator) => _mediator = mediator;

        [HttpGet("expedientes-inscritos")]
        public async Task<IActionResult> GetExpedientes([FromQuery] int? idPlaza)
        {
            var data = await _mediator.Send(new GetExpedientesInscritosQuery(idPlaza));
            return Ok(new { success = true, data });
        }

        [HttpPost("evaluar-inicial")]
        public async Task<IActionResult> EvaluarInicial([FromBody] EvaluarExpedienteCommand command)
        {
            var res = await _mediator.Send(command);
            return Ok(new { success = res });
        }
    }
}
