using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Commands
{
    public record ActualizarPerfilCommand(
        int IdPostulante,
        string Telefono,
        DateTime FechaNacimiento,
        int IdSexoCat,
        string Direccion,
        int IdTipoViaCat,             // Obligatorio
        string? NumeroVia,            // Opcional
        string? NumeroDepto,          // Opcional
        string? Interior,             // Opcional
        string? Manzana,              // Opcional
        string? Lote,                 // Opcional
        string? Kilometro,            // Opcional
        string? BlockEdificio,        // Opcional
        string? Etapa,                // Opcional
        int IdTipoZonaCat,            // Obligatorio
        string NombreZona,            // Obligatorio
        string IdUbigeoDistrito,      // Obligatorio
        string? ReferenciaDireccion   // Opcional
    ) : IRequest<bool>;
}
