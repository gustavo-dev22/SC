using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public record InfoAdicionalDto(
        int IdPostulante,
        bool DisponibilidadInterior,
        List<string> DepartamentosIds
    );
}
