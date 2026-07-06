using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Commands
{
    public record GuardarCalificacionCurricularCommand(
        int IdPostulacion,
        decimal NotaFormacion,
        decimal NotaCapacitacion,
        decimal NotaExperiencia
    ) : IRequest<bool>;
}
