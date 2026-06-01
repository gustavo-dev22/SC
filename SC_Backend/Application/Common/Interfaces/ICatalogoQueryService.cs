using Application.Catalogos.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ICatalogoQueryService
    {
        Task<List<CatalogoValorDto>> ObtenerValoresByTipoAsync(int idTipo, int pageNumber, int pageSize);
        Task<List<CatalogoTipoDto>> ObtenerTiposActivosAsync();
        Task<List<CatalogoValorDto>> ListarValoresByCodigoTipoAsync(string codigoTipo);
        Task<List<CentroEstudioDto>> ListarInstitutosPredictivoAsync(string filtro);
    }
}
