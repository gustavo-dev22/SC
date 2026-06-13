using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Postulantes.Dtos.FichaPostulante;

namespace Application.Postulantes.Dtos
{
    public class FichaReporteDto
    {
        public CabeceraPostulanteReporte Cabecera { get; set; } = new();
        public List<FormacionReporte> Formaciones { get; set; } = [];
        public List<ColegiaturaReporte> Colegiaturas { get; set; } = [];
        public List<IdiomaReporte> Idiomas { get; set; } = [];
        public List<OfimaticaReporte> Ofimaticas { get; set; } = [];
        public List<CertificacionReporte> Certificaciones { get; set; } = [];
        public List<ExperienciaReporte> Experiencias { get; set; } = [];
        public List<OtrosRequisitosReporte> OtrosRequisitos { get; set; } = [];
        public InfoAdicionalReporte InfoAdicional { get; set; } = new();
        public byte[]? FirmaBytes { get; set; }
    }
}
