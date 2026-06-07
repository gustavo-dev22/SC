using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Postulantes.Commands
{
    public record MantenimientoExperienciaCommand(
        string Accion, int IdExperiencia, int IdPostulante, string EmpresaInstitucion,
        string CargoPuesto, DateTime FechaInicio, DateTime? FechaFin, bool EsSectorPublico,
        bool EsExperienciaEspecifica, string FuncionesPrincipales, int IdSectorCat, int IdRegimenCat, int IdMotivoCambioCat, decimal RemuneracionMensual
    ) : IRequest<bool>;
}
