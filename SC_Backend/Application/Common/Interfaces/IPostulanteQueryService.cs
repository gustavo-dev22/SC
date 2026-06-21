using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Dtos;
using Application.Oportunidades.Dtos;
using Application.Postulantes.Dtos;

namespace Application.Common.Interfaces
{
    public interface IPostulanteQueryService
    {
        Task<dynamic> ObtenerByDocumentoAsync(string numDocumento);
        Task<PerfilPostulanteDto?> ObtenerPerfilByIdAsync(int idPostulante);
        Task<List<PostulanteFormacionDto>> ListarFormacionAsync(int idPostulante);
        Task<List<PostulanteCertificacionDto>> ListarCertificacionesAsync(int idPostulante);
        Task<List<PostulanteExperienciaDto>> ListarExperienciaAsync(int idPostulante);
        Task<bool> ExisteSuperposicionLaboralAsync(int idPostulante, int idExperiencia, DateTime fechaInicio, DateTime? fechaFin);
        Task<List<PostulanteColegiaturaDto>> ListarColegiaturasAsync(int idPostulante);
        Task<List<PostulanteIdiomaDto>> ListarIdiomasAsync(int idPostulante);
        Task<List<PostulanteOfimaticaDto>> ListarOfimaticaAsync(int idPostulante);
        Task<List<PostulanteRequisitoEspecialDto>> ListarRequisitosEspecialesAsync(int idPostulante);
        Task<AvanceCurriculumDto> ObtenerAvanceCurriculumAsync(int idPostulante);
        Task<List<UbigeoDto>> ObtenerDepartamentosAsync();
        Task<List<UbigeoDto>> ObtenerProvinciasAsync(string idDepartamento);
        Task<List<UbigeoDto>> ObtenerDistritosAsync(string idProvincia);
        Task<InfoAdicionalDto?> ObtenerInfoAdicionalAsync(int idPostulante);
        Task<PostulanteFirmaDto?> ObtenerFirmaAsync(int idPostulante);
        Task<byte[]> ObtenerFichaPdfAsync(int idPostulante);
        Task<List<PostulanteDeclaracionDto>> ListarDeclaracionesAsync(int idPostulante, int idTipo);
        Task<byte[]?> ObtenerFirmaBytesAsync(int idPostulante);
        Task<byte[]> ObtenerConstanciaPostulacionPdfAsync(MisPostulacionesDto datos, byte[]? firmaPostulanteBytes);
    }
}
