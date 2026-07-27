using Application.Comite.Queries;
using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Handlers
{
    public class GetActaDesiertaPdfQueryHandler : IRequestHandler<GetActaDesiertaPdfQuery, byte[]>
    {
        private readonly IComiteEvaluadorService _comiteService;

        public GetActaDesiertaPdfQueryHandler(IComiteEvaluadorService comiteService)
        {
            _comiteService = comiteService;
        }

        public async Task<byte[]> Handle(GetActaDesiertaPdfQuery request, CancellationToken cancellationToken)
        {
            return await _comiteService.ObtenerActaDesiertaPdfAsync(request.IdPlaza);
        }
    }
}
