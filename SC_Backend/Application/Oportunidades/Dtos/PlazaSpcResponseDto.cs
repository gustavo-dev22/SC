using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Oportunidades.Dtos
{
    public class PaginatedSpcResponseDto<T>
    {
        [JsonPropertyName("content")]
        public List<T> Content { get; set; } = [];

        [JsonPropertyName("totalElements")]
        public long TotalElements { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }
    }

    public class PlazaJavaDto
    {
        [JsonPropertyName("idConvocatoria")] 
        public int IdConvocatoria { get; set; }

        [JsonPropertyName("codigoConvocatoria")]
        public string CodigoConvocatoria { get; set; } = string.Empty;

        [JsonPropertyName("nombreOficina")]
        public string NombreOficina { get; set; } = string.Empty;

        [JsonPropertyName("cargo")] 
        public CargoJavaDto? Cargo { get; set; }

        [JsonPropertyName("sede")]
        public SedeJavaDto? Sede { get; set; }

        [JsonPropertyName("remuneracion")]
        public decimal? Remuneracion { get; set; }

        [JsonPropertyName("fechaFin")] 
        public DateTime? FechaFin { get; set; }

        [JsonPropertyName("fechaFinPostulacion")]
        public DateTime? FechaFinPostulacion { get; set; }
    }

    public class CargoJavaDto
    {
        [JsonPropertyName("nombreCargo")]
        public string NombreCargo { get; set; } = string.Empty;
    }

    public class SedeJavaDto
    {
        [JsonPropertyName("nombreSede")]
        public string NombreSede { get; set; } = string.Empty;
    }
}
