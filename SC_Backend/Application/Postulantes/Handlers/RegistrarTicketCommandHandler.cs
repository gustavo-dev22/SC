using Application.Common.Interfaces;
using Application.Postulantes.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Handlers
{
    public class RegistrarTicketCommandHandler : IRequestHandler<RegistrarTicketCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;

        public RegistrarTicketCommandHandler(IPostulanteCommandService commandService)
        {
            _commandService = commandService;
        }

        public async Task<bool> Handle(RegistrarTicketCommand request, CancellationToken cancellationToken)
        {
            // Validaciones básicas de negocio antes de persistir
            if (string.IsNullOrWhiteSpace(request.Asunto))
                throw new InvalidOperationException("El asunto de la solicitud no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(request.Descripcion))
                throw new InvalidOperationException("La descripción o descargo legal no puede estar vacía.");

            // Invocamos al servicio de infraestructura que ejecuta el Stored Procedure
            return await _commandService.InsertarTicketAsync(
                request.IdPostulante,
                request.IdPlaza,
                request.IdTipoTicketCat,
                request.Asunto,
                request.Descripcion
            );
        }
    }
}
