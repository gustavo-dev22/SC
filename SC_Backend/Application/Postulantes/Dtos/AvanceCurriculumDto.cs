using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Postulantes.Dtos
{
    public class AvanceCurriculumDto
    {
        public int PorcentajeTotal { get; set; }
        public bool TieneDatosPersonales { get; set; }
        public bool TieneFormacion { get; set; }
        public bool TieneColegiatura { get; set; }
        public bool TieneIdiomas { get; set; }
        public bool TieneOfimatica { get; set; }
        public bool TieneCertificacion { get; set; }
        public bool TieneExperiencia { get; set; }
        public bool TieneOtrosRequisitos { get; set; }
        public bool TieneInformacionAdicional { get; set; }
        public bool TieneFirma { get; set; }
    }
}
