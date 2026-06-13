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
    public class PostulanteFirmaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostulanteFirmaController(IMediator mediator) => _mediator = mediator;

        [HttpGet("firma/{idPostulante:int}")]
        public async Task<IActionResult> GetFirma(int idPostulante)
        {
            var result = await _mediator.Send(new ObtenerFirmaQuery(idPostulante));
            return Ok(BaseResponse<PostulanteFirmaDto>.Ok(result));
        }

        [HttpPost("firma/subir")]
        [Consumes("multipart/form-data")] // Especifica el formato de archivo por red
        public async Task<IActionResult> SubirFirma([FromForm] int idPostulante, [FromForm] IFormFile archivo)
        {
            var result = await _mediator.Send(new GuardarFirmaCommand(idPostulante, archivo));
            return Ok(BaseResponse<bool>.Ok(result, "Su firma manuscrita ha sido digitalizada y guardada con éxito."));
        }
    }
}
