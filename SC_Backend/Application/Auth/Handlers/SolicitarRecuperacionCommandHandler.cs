using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Auth.Commands;
using Application.Auth.Dtos;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Auth.Handlers
{
    public class SolicitarRecuperacionCommandHandler : IRequestHandler<SolicitarRecuperacionCommand, SolicitudRecuperacionResultDto>
    {
        private readonly IPostulanteCommandService _commandService;

        public SolicitarRecuperacionCommandHandler(IPostulanteCommandService commandService)
        {
            _commandService = commandService;
        }

        public async Task<SolicitudRecuperacionResultDto> Handle(SolicitarRecuperacionCommand request, CancellationToken cancellationToken)
        {
            // Generamos un token pseudo-aleatorio único libre de caracteres raros
            string token = Convert.ToHexString(Guid.NewGuid().ToByteArray());

            // Delegamos la persistencia a la Infraestructura
            DatosPostulanteTokenDto? postulante = await _commandService.RegistrarTokenRecuperacionAsync(request.NumDocumento, token);

            if (postulante == null)
            {
                return new SolicitudRecuperacionResultDto
                {
                    Success = false,
                    Message = "El número de documento ingresado no se encuentra registrado en el sistema."
                };
            }

            // Construimos el enlace dinámico que Angular atrapará
            string linkUrl = $"http://localhost:4200/auth/restablecer-password?token={token}";

            return new SolicitudRecuperacionResultDto
            {
                Success = true,
                Message = $"Se generó el acceso de restablecimiento para {postulante.Nombres}.",
                LinkDesarrollo = linkUrl // 🚀 Viaja de retorno al Front para usarlo directo en local sin configurar correos
            };
        }
    }
}
