using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Commands
{
    public record MantenimientoFormacionCommand(
        string Accion, int IdFormacion, int IdPostulante, int IdNivelCat, int IdEstadoCat,
        string Institucion, string Carrera, int MesInicio, int AnioInicio, int? MesFin, int? AnioFin, string? RutaSustento
    ) : IRequest<bool>;
}
