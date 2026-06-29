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
    public class EliminarSustentoFormacionCommandHandler : IRequestHandler<EliminarSustentoFormacionCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;
        private readonly IFileStorageService _storageService;

        public EliminarSustentoFormacionCommandHandler(IPostulanteCommandService commandService, IFileStorageService storageService)
        {
            _commandService = commandService;
            _storageService = storageService;
        }

        public async Task<bool> Handle(EliminarSustentoFormacionCommand request, CancellationToken cancellationToken)
        {
            // 1. Limpiamos SQL Server y recuperamos el path físico del archivo obsoleto
            string rutaArchivoAntiguo = await _commandService.EliminarRutaSustentoAsync(request.IdFormacion);

            // 2. Borramos el archivo del servidor para no acumular basura 🧹
            if (!string.IsNullOrEmpty(rutaArchivoAntiguo))
            {
                _storageService.EliminarArchivoFisico(rutaArchivoAntiguo);
                return true;
            }
            return false;
        }
    }
}
