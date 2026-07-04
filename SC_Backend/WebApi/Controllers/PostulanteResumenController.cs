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
        // 🚀 NUEVO: Añadimos [FromQuery] int? idPlaza para capturar el parámetro opcional desde Angular
        public async Task<IActionResult> ObtenerEstadoPostulacionActual([FromQuery] int? idPlaza)
        {
            string userIdClaim = string.Empty;

            // 1. Extraemos manualmente el Header de Autorización de la petición HTTP
            string authHeader = Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                // 2. Limpiamos el prefijo 'Bearer ' para quedarnos con el token puro ("POSTULANTE-1-...")
                string tokenCrudo = authHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    // 3. 🎯 AQUÍ APLICAS TU LÓGICA DE NEGOCIO PARA EXTRAER EL ID:
                    var partes = tokenCrudo.Split('-');
                    if (partes.Length >= 2 && partes[0] == "POSTULANTE")
                    {
                        userIdClaim = partes[1]; // Captura el "1" de forma dinámica
                    }
                    else
                    {
                        // Si tu token está encriptado en Base64 plano en los headers, lo decodificamos primero:
                        byte[] datosBytes = System.Convert.FromBase64String(tokenCrudo);
                        string tokenDecodificado = System.Text.Encoding.UTF8.GetString(datosBytes);

                        var partesDecodificadas = tokenDecodificado.Split('-');
                        if (partesDecodificadas.Length >= 2)
                        {
                            userIdClaim = partesDecodificadas[1];
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"🔴 Error al procesar el token manual: {ex.Message}");
                }
            }

            // 4. Si tras revisar el header no pudimos rescatar el ID del usuario, abortamos
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest(new { success = false, message = "No se pudo identificar una sesión de postulante válida." });
            }

            // 5. Despachamos el Query a MediatR enviando TANTO el userId como el idPlaza 🎯
            var idEstado = await _mediator.Send(new GetEstadoPostulacionActualQuery(int.Parse(userIdClaim), idPlaza));

            // 🚀 IMPRIMIR EN CONSOLA DEL BACKEND (Ahora con soporte de plazas)
            System.Console.WriteLine($"====================================================");
            System.Console.WriteLine($"🔍 DEBUG MANUAL: El ID extraído del Token es: {userIdClaim}");
            System.Console.WriteLine($"🏢 DEBUG MANUAL: El ID de Plaza evaluado es: {idPlaza?.ToString() ?? "NULL (Última)"}");
            System.Console.WriteLine($"🎯 DEBUG MANUAL: El ID del Estado es: {idEstado ?? 0}");
            System.Console.WriteLine($"====================================================");

            return Ok(new { success = true, data = idEstado ?? 0 });
        }
    }
}
