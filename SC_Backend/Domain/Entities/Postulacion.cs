using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Postulacion
    {
        public int IdPostulante { get; private set; }
        public int IdPlaza { get; private set; }
        public DateTime FechaFinConvocatoria { get; private set; }
        public bool YaPostuloAEstaConvocatoria { get; private set; }

        public Postulacion(int idPostulante, int idPlaza, DateTime fechaFinConvocatoria, bool yaPostulo)
        {
            IdPostulante = idPostulante;
            IdPlaza = idPlaza;
            FechaFinConvocatoria = fechaFinConvocatoria;
            YaPostuloAEstaConvocatoria = yaPostulo;
        }

        // 🚀 LÓGICA DE DOMINIO: Valida si la operación cumple con las leyes del negocio
        public (bool EsValido, string Mensaje) ValidarReglasDePostulacion()
        {
            if (YaPostuloAEstaConvocatoria)
                return (false, "Usted ya cuenta con una postulación registrada y activa para este proceso de selección.");

            // Limpiamos las horas para comparar puramente las fechas del calendario
            if (FechaFinConvocatoria.Date < DateTime.Now.Date)
                return (false, "El plazo legal establecido para postular a esta convocatoria ya ha finalizado.");

            return (true, "Validación de dominio exitosa.");
        }

        // 🚀 MÉTODO DE DOMINIO: Genera de forma automatizada el código único de expediente
        public string GenerarCodigoCorrelativo(int totalPostulacionesPrevias)
        {
            int siguienteNumero = totalPostulacionesPrevias + 1;
            return $"POST-{DateTime.Now.Year}-{siguienteNumero.ToString().PadLeft(5, '0')}";
        }
    }
}
