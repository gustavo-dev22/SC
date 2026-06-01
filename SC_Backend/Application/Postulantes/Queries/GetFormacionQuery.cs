using Application.Postulantes.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Queries
{
    public record GetFormacionQuery(int IdPostulante) : IRequest<List<PostulanteFormacionDto>>;
}
