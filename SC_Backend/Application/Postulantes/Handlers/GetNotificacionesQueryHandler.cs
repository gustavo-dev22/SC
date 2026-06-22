using Application.Common.Interfaces;
using Application.Postulantes.Dtos;
using Application.Postulantes.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Handlers
{
    public class GetNotificacionesQueryHandler : IRequestHandler<GetNotificacionesQuery, List<NotificacionDto>>
    {
        private readonly IPostulanteQueryService _queryService;

        public GetNotificacionesQueryHandler(IPostulanteQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<List<NotificacionDto>> Handle(GetNotificacionesQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerNotificacionesAsync(request.IdPostulante);
        }
    }
}
