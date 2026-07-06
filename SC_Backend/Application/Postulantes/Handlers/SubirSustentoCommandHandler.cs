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
    public class SubirSustentoCommandHandler : IRequestHandler<SubirSustentoCommand, bool>
    {
        private readonly IPostulanteCommandService _postulanteService;
        private readonly IFileStorageService _fileStorageService;

        public SubirSustentoCommandHandler(IPostulanteCommandService postulanteService, IFileStorageService fileStorageService)
        {
            _postulanteService = postulanteService;
            _fileStorageService = fileStorageService;
        }

        public async Task<bool> Handle(SubirSustentoCommand request, CancellationToken cancellationToken)
        {
            if (request.Archivo == null || request.Archivo.Length == 0) return false;

            using var stream = request.Archivo.OpenReadStream();

            // 1. Limpiamos la base de datos y recuperamos ruta vieja usando la sección del comando
            string rutaAntiguaParaLimpiar = await _postulanteService.EliminarRutaSustentoAsync(request.IdRegistro, request.Seccion);
            if (!string.IsNullOrEmpty(rutaAntiguaParaLimpiar))
            {
                _fileStorageService.EliminarArchivoFisico(rutaAntiguaParaLimpiar);
            }

            // 2. Guardamos físicamente en disco el nuevo archivo
            string urlRelativaCompleta = await _fileStorageService.GuardarArchivoAsync(
                stream,
                request.Archivo.FileName,
                "sustentos",
                cancellationToken
            );

            if (string.IsNullOrEmpty(urlRelativaCompleta)) return false;

            // 3. Persistimos la nueva URL pasando la sección dinámica
            return await _postulanteService.ActualizarRutaSustentoAsync(request.IdRegistro, urlRelativaCompleta, request.Seccion);
        }
    }
}
