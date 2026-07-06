using Application.Comite.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Queries
{
    public record GetCalificacionCurricularQuery(int IdPlaza) : IRequest<List<CalificacionCurricularDto>>;
}
