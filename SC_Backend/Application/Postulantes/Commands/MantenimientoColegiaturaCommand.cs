using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Postulantes.Commands
{
    public record MantenimientoColegiaturaCommand(
        string Accion, int IdColegiatura, int IdPostulante, int IdColegioCat,
        string NumeroColegiacion, DateTime FechaColegiacion, string? CertificadoHabilitacionRuta
    ) : IRequest<bool>;
}
