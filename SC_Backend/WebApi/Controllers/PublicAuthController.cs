using Application.Common.Dtos;
using Application.Postulantes.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/public/auth")]
    public class PublicAuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PublicAuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("registro")]
        public async Task<IActionResult> RegistrarPostulante([FromBody] RegistrarPostulanteCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(BaseResponse<bool>.Ok(result, "Su cuenta de postulante fue creada con éxito. Ya puede iniciar sesión."));
            }
            catch (SqlException ex) when (ex.Number == 52000)
            {
                return BadRequest(BaseResponse<bool>.Fail(ex.Message));
            }
        }
    }
}
