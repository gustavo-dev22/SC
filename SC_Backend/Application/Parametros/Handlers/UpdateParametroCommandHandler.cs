using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Parametros.Commands;
using MediatR;

namespace Application.Parametros.Handlers
{
    public class UpdateParametroCommandHandler : IRequestHandler<UpdateParametroCommand, bool>
    {
        private readonly IParametroCommandService _commandService;
        public UpdateParametroCommandHandler(IParametroCommandService srv) => _commandService = srv;

        public async Task<bool> Handle(UpdateParametroCommand request, CancellationToken token)
        {
            return await _commandService.ActualizarParametroAsync(request.Codigo, request.Valor);
        }
    }
}
