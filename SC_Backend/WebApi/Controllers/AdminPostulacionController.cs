using Application.Admin.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminPostulacionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminPostulacionController(IMediator mediator) => _mediator = mediator;

        [HttpGet("trazabilidad")]
        public async Task<IActionResult> GetTrazabilidad([FromQuery] string codigoExpediente)
        {
            if (string.IsNullOrWhiteSpace(codigoExpediente))
                return BadRequest(new { message = "El código de expediente es requerido." });

            var data = await _mediator.Send(new GetTrazabilidadQuery(codigoExpediente));
            return Ok(new { success = true, data });
        }
    }
}
