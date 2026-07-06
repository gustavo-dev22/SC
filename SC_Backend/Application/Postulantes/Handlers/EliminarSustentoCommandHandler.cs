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
    public class EliminarSustentoCommandHandler : IRequestHandler<EliminarSustentoCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;
        private readonly IFileStorageService _storageService;

        public EliminarSustentoCommandHandler(IPostulanteCommandService commandService, IFileStorageService storageService)
        {
            _commandService = commandService;
            _storageService = storageService;
        }

        public async Task<bool> Handle(EliminarSustentoCommand request, CancellationToken cancellationToken)
        {
            // 1. Limpiamos base de datos pasando la sección respectiva
            string rutaArchivoAntiguo = await _commandService.EliminarRutaSustentoAsync(request.IdRegistro, request.Seccion);

            // 2. Borramos archivo del disco
            if (!string.IsNullOrEmpty(rutaArchivoAntiguo))
            {
                _storageService.EliminarArchivoFisico(rutaArchivoAntiguo);
                return true;
            }
            return false;
        }
    }
}
