using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Oportunidades.Dtos
{
    // 🚀 Envoltorio de paginación estándar de Spring Boot
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

    // 🚀 DTO que calza milimétricamente con las propiedades que escupe el API de Java
    public class PlazaJavaDto
    {
        [JsonPropertyName("idConvocatoria")] // 🚀 Cambiado: Ahora captura el ID real de la convocatoria
        public int IdConvocatoria { get; set; }

        [JsonPropertyName("codigoConvocatoria")]
        public string CodigoConvocatoria { get; set; } = string.Empty;

        [JsonPropertyName("nombreOficina")]
        public string NombreOficina { get; set; } = string.Empty;

        [JsonPropertyName("cargo")] // 🚀 NUEVO: Mapea el objeto anidado del puesto
        public CargoJavaDto? Cargo { get; set; }

        [JsonPropertyName("sede")] // 🚀 NUEVO: Mapea el objeto anidado de la infraestructura
        public SedeJavaDto? Sede { get; set; }

        [JsonPropertyName("remuneracion")]
        public decimal? Remuneracion { get; set; }

        [JsonPropertyName("fechaFin")] // 🚀 Cambiado: Sincronizado con "fechaFin"
        public DateTime? FechaFin { get; set; }
    }

    // 🚀 Clases espejo para capturar los nodos secundarios de Java
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
