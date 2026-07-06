using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Comite.Dtos;

namespace Application.Common.Interfaces
{
    public interface IComiteEvaluadorService
    {
        Task<List<ExpedienteInscritoDto>> ListarExpedientesInscritosAsync(int? idPlaza);
        Task<bool> EvaluarExpedienteInicialAsync(int idPostulacion, bool aprobado, string observacion);
        Task<byte[]> ObtenerActaInicialPdfAsync(List<ExpedienteInscritoDto> expedientes, string codigoConvocatoria, string nombrePuesto);
        Task<List<EvaluacionConocimientosDto>> ListarEvaluacionConocimientosAsync(int idPlaza);
        Task<bool> RegistrarNotaConocimientosAsync(int idPostulacion, decimal nota);
        Task<byte[]> ObtenerActaConocimientosPdfAsync(List<EvaluacionConocimientosDto> candidatos, string codigoConvocatoria, string nombrePuesto);
        Task<List<CalificacionCurricularDto>> ListarCandidatosCurricularAsync(int idPlaza);
        Task<bool> RegistrarCalificacionCurricularAsync(int idPostulacion, decimal notaFormacion, decimal notaCapacitacion, decimal notaExperiencia);
        Task<byte[]> ObtenerActaCurricularPdfAsync(List<CalificacionCurricularDto> candidatos, string codigoConvocatoria, string nombrePuesto);
    }
}
