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
    public class MantenimientoExperienciaCommandHandler : IRequestHandler<MantenimientoExperienciaCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;
        private readonly IPostulanteQueryService _queryService;

        public MantenimientoExperienciaCommandHandler(IPostulanteCommandService commandService, IPostulanteQueryService queryService)
        {
            _commandService = commandService;
            _queryService = queryService;
        }

        public async Task<bool> Handle(MantenimientoExperienciaCommand request, CancellationToken cancellationToken)
        {
            // 1. BLINDAJE: Solo validamos cruces si la intención es registrar o modificar
            if (request.Accion == "REGISTRAR" || request.Accion == "MODIFICAR")
            {
                bool seCruza = await _queryService.ExisteSuperposicionLaboralAsync(
                    request.IdPostulante,
                    request.IdExperiencia,
                    request.FechaInicio,
                    request.FechaFin
                );

                if (seCruza)
                {
                    // Al lanzar la excepción aquí, el flujo se rompe de inmediato, 
                    // protegiendo la base de datos de registros duplicados o corruptos.
                    throw new ApplicationException("El periodo laboral ingresado colisiona con otra experiencia ya registrada en el sistema.");
                }
            }

            return await _commandService.MantenimientoExperienciaAsync(request);
        }
    }
}
