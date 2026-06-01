using Application.Postulantes.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IPostulanteQueryService
    {
        Task<dynamic> ObtenerByDocumentoAsync(string numDocumento);
        Task<PerfilPostulanteDto?> ObtenerPerfilByIdAsync(int idPostulante);
        Task<List<PostulanteFormacionDto>> ListarFormacionAsync(int idPostulante);
    }
}
