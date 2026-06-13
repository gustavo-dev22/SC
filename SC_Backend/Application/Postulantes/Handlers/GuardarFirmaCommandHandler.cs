using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Postulantes.Commands;
using MediatR;

namespace Application.Postulantes.Handlers
{
    public class GuardarFirmaCommandHandler : IRequestHandler<GuardarFirmaCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;
        public GuardarFirmaCommandHandler(IPostulanteCommandService commandService) => _commandService = commandService;

        public async Task<bool> Handle(GuardarFirmaCommand request, CancellationToken cancellationToken)
        {
            if (request.Archivo == null || request.Archivo.Length == 0) return false;

            using var memoryStream = new MemoryStream();
            await request.Archivo.CopyToAsync(memoryStream, cancellationToken);
            byte[] bytes = memoryStream.ToArray();

            return await _commandService.GuardarFirmaAsync(request.IdPostulante, bytes, request.Archivo.ContentType);
        }
    }
}
