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
    }
}
