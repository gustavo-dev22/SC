using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Dtos
{
    public class CuadroMeritoFinalDto
    {
        public int IdPostulacion { get; set; }
        public string CodigoPostulacionUnid { get; set; } = null!;
        public string PostulanteNombre { get; set; } = null!;
        public int IdEstadoPostulacionCat { get; set; }

        public bool? FaseExpedientesAprobado { get; set; }
        public bool? FaseConocimientosAprobado { get; set; }
        public decimal? NotaConocimientos { get; set; }
        public decimal? PtjePonderadoConocimientos { get; set; }

        public bool? FaseCurricularAprobado { get; set; }
        public decimal? NotaFormacion { get; set; }
        public decimal? NotaCapacitacion { get; set; }
        public decimal? NotaExperiencia { get; set; }
        public decimal? NotaCurricularFinal { get; set; }
        public decimal? PtjePonderadoCurricular { get; set; }

        public bool? FaseEntrevistaAprobado { get; set; }
        public decimal? NotaEntrevista { get; set; }
        public decimal? PtjePonderadoEntrevista { get; set; }

        public decimal PtjeBaseAcumulado { get; set; }
        public bool TieneConadis { get; set; }
        public bool TieneFFAA { get; set; }
        public int PorcentajeDecan { get; set; }

        public decimal PtjeBonifFFAA { get; set; }
        public decimal PtjeBonifConadis { get; set; }
        public decimal PtjeBonifDecan { get; set; }

        public decimal? NotaFinalAcumulada { get; set; }
        public string SituacionFinalDesc { get; set; } = null!;
    }
}
