using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public record PostulanteFirmaDto(
        int IdPostulante,
        string? FirmaBase64,
        string? FirmaTipoMime
    );
}
