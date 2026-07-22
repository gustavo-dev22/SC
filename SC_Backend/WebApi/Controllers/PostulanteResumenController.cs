using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Common.Dtos;
using Application.Postulantes.Dtos;
using Application.Postulantes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] int idPostulante)
        {
            var data = await _mediator.Send(new GetPostulanteDashboardQuery(idPostulante));
            return Ok(new { success = true, data });
        }

        [HttpGet("estado-actual")]
        [Authorize]
        public async Task<IActionResult> ObtenerEstadoPostulacionActual([FromQuery] int? idPlaza)
        {
            string userIdClaim = string.Empty;

            userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("nameid")?.Value
                       ?? string.Empty;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                string authHeader = Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    string tokenJwt = authHeader.Substring("Bearer ".Length).Trim();
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        if (handler.CanReadToken(tokenJwt))
                        {
                            var jwtToken = handler.ReadJwtToken(tokenJwt);
                            userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                                c.Type == ClaimTypes.NameIdentifier ||
                                c.Type == "nameid" ||
                                c.Type == "sub")?.Value ?? string.Empty;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine($"Error leyendo JWT: {ex.Message}");
                    }
                }
            }

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest(new { success = false, message = "No se pudo identificar una sesión de postulante válida." });
            }

            int idPostulante = int.Parse(userIdClaim);
            var idEstado = await _mediator.Send(new GetEstadoPostulacionActualQuery(idPostulante, idPlaza));

            System.Console.WriteLine($"====================================================");
            System.Console.WriteLine($"🔍 DEBUG JWT: El ID del Postulante resuelto es: {idPostulante}");
            System.Console.WriteLine($"🏢 DEBUG JWT: El ID de Plaza evaluado es: {idPlaza?.ToString() ?? "NULL (Última)"}");
            System.Console.WriteLine($"🎯 DEBUG JWT: El ID del Estado es: {idEstado ?? 0}");
            System.Console.WriteLine($"====================================================");

            return Ok(new { success = true, data = idEstado ?? 0 });
        }
    }
}
