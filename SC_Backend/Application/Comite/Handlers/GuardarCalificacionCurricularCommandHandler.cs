using Application.Comite.Commands;
using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Handlers
{
    public class GuardarCalificacionCurricularCommandHandler : IRequestHandler<GuardarCalificacionCurricularCommand, bool>
    {
        private readonly IComiteEvaluadorService _comiteService;

        public GuardarCalificacionCurricularCommandHandler(IComiteEvaluadorService comiteService)
        {
            _comiteService = comiteService;
        }

        public async Task<bool> Handle(GuardarCalificacionCurricularCommand request, CancellationToken cancellationToken)
        {
            return await _comiteService.RegistrarCalificacionCurricularAsync(
                request.IdPostulacion,
                request.NotaFormacion,
                request.NotaCapacitacion,
                request.NotaExperiencia
            );
        }
    }
}
