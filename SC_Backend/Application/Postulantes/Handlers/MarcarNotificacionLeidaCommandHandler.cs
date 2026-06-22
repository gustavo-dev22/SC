using Application.Common.Interfaces;
using Application.Postulantes.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Handlers
{
    public class MarcarNotificacionLeidaCommandHandler : IRequestHandler<MarcarNotificacionLeidaCommand, bool>
    {
        private readonly IPostulanteQueryService _queryService;

        public MarcarNotificacionLeidaCommandHandler(IPostulanteQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<bool> Handle(MarcarNotificacionLeidaCommand request, CancellationToken cancellationToken)
        {
            return await _queryService.MarcarNotificacionLeidaAsync(request.IdNotificacion);
        }
    }
}
