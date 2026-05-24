using Application.Auth.Commands;
using Application.Auth.Dto;
using Application.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            // El controlador simplemente reenvía el Command a través del bus de MediatR
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.Success)
            {
                // Retornamos una respuesta unificada con estructura estándar de fallo corporativo
                return BadRequest(BaseResponse<LoginResultDto>.Fail("Usuario, contraseña o rol de acceso incorrecto."));
            }

            // Retornamos el DTO de autenticación unificado envuelto en nuestro BaseResponse genérico
            return Ok(BaseResponse<LoginResultDto>.Ok(result, "Autenticación concedida con éxito."));
        }
    }
}
