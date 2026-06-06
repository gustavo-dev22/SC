using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Postulantes.Commands
{
    public record MantenimientoIdiomaCommand(
        string Accion, int IdPostulanteIdioma, int IdPostulante, int IdIdiomaCat,
        int IdNivelHablaCat, int IdNivelLecturaCat, int IdNivelEscrituraCat
    ) : IRequest<bool>;
}
