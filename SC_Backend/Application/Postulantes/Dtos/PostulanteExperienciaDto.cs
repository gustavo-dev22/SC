using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class PostulanteExperienciaDto
    {
        public int IdExperiencia { get; set; }
        public int IdPostulante { get; set; }
        public string EmpresaInstitucion { get; set; } = string.Empty;
        public string CargoPuesto { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool EsSectorPublico { get; set; }
        public bool EsExperienciaEspecifica { get; set; }
        public string FuncionesPrincipales { get; set; } = string.Empty;
        public int IdSectorCat { get; set; }
        public string SectorClasificacion { get; set; } = string.Empty;
        public int IdRegimenCat { get; set; }
        public string RegimenClasificacion { get; set; } = string.Empty;
        public int IdMotivoCambioCat { get; set; }
        public string MotivoCambioClasificacion { get; set; } = string.Empty;
        public decimal RemuneracionMensual { get; set; }
        public string? RutaSustento { get; set; }

        // 🚀 Lógica de Cómputo de Tiempo en el Modelo de Presentación de Aplicación
        public int TotalDiasAcumulados
        {
            get
            {
                DateTime limiteFin = FechaFin ?? DateTime.Now; // Si es null, es "Hasta la Actualidad" (Hoy)
                if (limiteFin < FechaInicio) return 0;

                return (limiteFin - FechaInicio).Days + 1; // Incluye el día de inicio en el cómputo
            }
        }
    }
}
