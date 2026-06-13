using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Postulantes.Dtos;
using MediatR;

namespace Application.Postulantes.Commands
{
    public record GuardarDeclaracionesCommand(int IdPostulante, List<GuardarDeclaracionItemDto> Declaraciones) : IRequest<bool>;
}
