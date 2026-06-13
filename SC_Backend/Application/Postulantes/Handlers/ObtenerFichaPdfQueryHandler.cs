using Application.Common.Interfaces;
using Application.Postulantes.Queries;
using MediatR;

namespace Application.Postulantes.Handlers
{
    public class ObtenerFichaPdfQueryHandler : IRequestHandler<ObtenerFichaPdfQuery, byte[]>
    {
        private readonly IPostulanteQueryService _queryService;

        public ObtenerFichaPdfQueryHandler(IPostulanteQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<byte[]> Handle(ObtenerFichaPdfQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.ObtenerFichaPdfAsync(request.IdPostulante);
        }
    }
}
