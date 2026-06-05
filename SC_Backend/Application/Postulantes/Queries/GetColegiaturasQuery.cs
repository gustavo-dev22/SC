using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Postulantes.Dtos;
using MediatR;

namespace Application.Postulantes.Queries
{
    public record GetColegiaturasQuery(int IdPostulante) : IRequest<List<PostulanteColegiaturaDto>>;
}
