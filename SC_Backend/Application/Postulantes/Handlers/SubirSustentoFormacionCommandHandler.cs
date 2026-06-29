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
    public class SubirSustentoFormacionCommandHandler : IRequestHandler<SubirSustentoFormacionCommand, bool>
    {
        private readonly IPostulanteCommandService _postulanteService;
        private readonly IFileStorageService _fileStorageService;

        public SubirSustentoFormacionCommandHandler(IPostulanteCommandService postulanteService, IFileStorageService fileStorageService)
        {
            _postulanteService = postulanteService;
            _fileStorageService = fileStorageService;
        }

        public async Task<bool> Handle(SubirSustentoFormacionCommand request, CancellationToken cancellationToken)
        {
            if (request.Archivo == null || request.Archivo.Length == 0) return false;

            // 1. Abrimos el stream binario del archivo de forma segura
            using var stream = request.Archivo.OpenReadStream();

            string rutaAntiguaParaLimpiar = await _postulanteService.EliminarRutaSustentoAsync(request.IdFormacion);
            if (!string.IsNullOrEmpty(rutaAntiguaParaLimpiar))
            {
                _fileStorageService.EliminarArchivoFisico(rutaAntiguaParaLimpiar); // Adios archivo viejo
            }

            // 2. Delegamos a la infraestructura el guardado físico en disco
            string urlRelativaCompleta = await _fileStorageService.GuardarArchivoAsync(
                stream,
                request.Archivo.FileName,
                "sustentos",
                cancellationToken
            );

            if (string.IsNullOrEmpty(urlRelativaCompleta)) return false;

            // 3. Guardamos la ruta en SQL Server mediante tu servicio de comandos convencional
            return await _postulanteService.ActualizarRutaSustentoAsync(request.IdFormacion, urlRelativaCompleta);
        }
    }
}
