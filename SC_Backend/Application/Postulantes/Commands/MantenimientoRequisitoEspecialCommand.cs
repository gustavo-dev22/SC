using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Postulantes.Commands
{
    public record MantenimientoRequisitoEspecialCommand(
        string Accion, int IdRequisitoEspecial, int IdPostulante, int IdTipoRequisitoCat,
        string DescripcionDocumento, string NumeroRegistro, DateTime? FechaEmision, DateTime? FechaVencimiento
    ) : IRequest<bool>;
}
