using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Comite.Dtos;
using MediatR;

namespace Application.Comite.Queries
{
    public record GetEvaluacionConocimientosQuery(int IdPlaza) : IRequest<List<EvaluacionConocimientosDto>>;
}
