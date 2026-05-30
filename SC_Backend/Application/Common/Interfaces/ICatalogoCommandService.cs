using Application.Catalogos.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ICatalogoCommandService
    {
        Task<bool> ProcesarMantenimientoAsync(CatalogoCommand command);
        Task<bool> ProcesarMantenimientoTipoAsync(TipoCommand command);
    }
}
