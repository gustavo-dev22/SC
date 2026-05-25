using MediatR;
using Application.Catalogos.Commands;
using Application.Common.Interfaces;

namespace Application.Catalogos.Handlers
{
    public class CatalogoCommandHandler : IRequestHandler<CatalogoCommand, bool>
    {
        private readonly ICatalogoCommandService _commandService;

        public CatalogoCommandHandler(ICatalogoCommandService commandService)
        {
            _commandService = commandService;
        }

        public async Task<bool> Handle(CatalogoCommand request, CancellationToken cancellationToken)
        {
            return await _commandService.ProcesarMantenimientoAsync(request);
        }
    }
}
