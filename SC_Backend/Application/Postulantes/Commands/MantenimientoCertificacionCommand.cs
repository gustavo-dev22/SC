using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Postulantes.Commands
{
    public record MantenimientoCertificacionCommand(
        string Accion, int IdCertificacion, int IdPostulante, int IdTipoEstudioCat,
        string NombreEstudio, string Institucion, int HorasAcademicas, DateTime FechaEmision, string? RutaSustento
    ) : IRequest<bool>;
}
