using Application.Comite.Commands;
using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Handlers
{
    public class DeclararPlazaDesiertaCommandHandler : IRequestHandler<DeclararPlazaDesiertaCommand, bool>
    {
        private readonly IComiteEvaluadorService _comiteService;

        public DeclararPlazaDesiertaCommandHandler(IComiteEvaluadorService comiteService)
        {
            _comiteService = comiteService;
        }

        public async Task<bool> Handle(DeclararPlazaDesiertaCommand request, CancellationToken cancellationToken)
        {
            return await _comiteService.DeclararPlazaDesiertaAsync(
                request.IdPlaza,
                request.IdMotivoDesiertaCat,
                request.SustentoDesierta,
                request.UsuarioDeclara
            );
        }
    }
}
