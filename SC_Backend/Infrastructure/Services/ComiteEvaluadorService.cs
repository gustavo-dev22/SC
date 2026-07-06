using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Comite.Dtos;
using Application.Common.Interfaces;
using Dapper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Services
{
    public class ComiteEvaluadorService : IComiteEvaluadorService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ComiteEvaluadorService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<List<ExpedienteInscritoDto>> ListarExpedientesInscritosAsync(int? idPlaza)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var result = await connection.QueryAsync<ExpedienteInscritoDto>(
                "sp_Comite_ListarExpedientesInscritos",
                new { IdPlaza = idPlaza },
                commandType: CommandType.StoredProcedure
            );
            return result.ToList();
        }

        public async Task<bool> EvaluarExpedienteInicialAsync(int idPostulacion, bool aprobado, string observacion)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var rowsAffected = await connection.QueryFirstOrDefaultAsync<int>(
                "sp_Comite_EvaluarExpedienteInicial",
                new { IdPostulacion = idPostulacion, Aprobado = aprobado, Observacion = observacion },
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<byte[]> ObtenerActaInicialPdfAsync(List<ExpedienteInscritoDto> expedientes, string codigoConvocatoria, string nombrePuesto)
        {
            // Memoria pura. QuestPDF procesa directamente los argumentos del Handler
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9.5f).FontFamily("Arial"));

                    // CABECERA DEL INFORME
                    page.Header().Column(column =>
                    {
                        column.Item().Text("SISTEMA DE CONVOCATORIAS DE PERSONAL (SCP)")
                            .FontSize(12).Bold().FontColor(Colors.Blue.Darken3);

                        column.Item().Text("ACTA DE EVALUACIÓN PRELIMINAR - REQUISITOS MÍNIMOS OBLIGATORIOS")
                            .FontSize(10).Bold().FontColor(Colors.Grey.Darken2);

                        column.Item().PaddingTop(4).LineHorizontal(0.8f).LineColor(Colors.Grey.Lighten1);

                        column.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(cd => { cd.ConstantColumn(90); cd.RelativeColumn(); });

                            table.Cell().PaddingVertical(2).Text("Convocatoria:").Bold();
                            table.Cell().PaddingVertical(2).Text(codigoConvocatoria); // 🚀 Viene limpio desde el argumento

                            table.Cell().PaddingVertical(2).Text("Puesto CAS:").Bold();
                            table.Cell().PaddingVertical(2).Text(nombrePuesto); // 🚀 Viene limpio desde el argumento
                        });

                        column.Item().PaddingTop(12);
                    });

                    // CUERPO / TABLA DE EVALUADOS
                    page.Content().Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cd =>
                            {
                                cd.ConstantColumn(30);  // N°
                                cd.ConstantColumn(120); // Código Expediente
                                cd.RelativeColumn();    // Postulante
                                cd.ConstantColumn(120); // Resultado / Condición
                            });

                            table.Header(h =>
                            {
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("N°").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("N° Expediente").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Apellidos y Nombres").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Condición").Bold();
                            });

                            int contador = 1;
                            foreach (var p in expedientes)
                            {
                                // 🚀 SOLUCIÓN HISTÓRICA: Si el estado actual es 1108, 19 o 1109, significa que en esta fase de papeles SÍ FUE APTO.
                                string condicionText = p.IdEstadoPostulacionCat == 18 ? "PENDIENTE" :
                                                       (p.IdEstadoPostulacionCat == 1108 || p.IdEstadoPostulacionCat == 19 || p.IdEstadoPostulacionCat == 1109) ? "APTO" : "NO APTO";

                                // De igual manera, asignamos el color verde a todos los que pasaron legítimamente esta primera etapa
                                var colorCondicion = (p.IdEstadoPostulacionCat == 1108 || p.IdEstadoPostulacionCat == 19 || p.IdEstadoPostulacionCat == 1109)
                                                     ? Colors.Green.Darken2
                                                     : (p.IdEstadoPostulacionCat == 20 ? Colors.Red.Darken2 : Colors.Orange.Darken2);

                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(4).Text(contador.ToString());
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(4).Text(p.CodigoPostulacionUnid);
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(4).Text(p.PostulanteNombre);
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(4).Text(condicionText).Bold().FontColor(colorCondicion);

                                contador++;
                            }
                        });
                    });

                    // PIE DE PÁGINA
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(" de ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();

            return pdfBytes;
        }

        public async Task<List<EvaluacionConocimientosDto>> ListarEvaluacionConocimientosAsync(int idPlaza)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var resultado = await connection.QueryAsync<EvaluacionConocimientosDto>(
                "sp_Comite_ListarEvaluacionConconocimientos",
                new { IdPlaza = idPlaza },
                commandType: CommandType.StoredProcedure
            );

            return resultado.ToList();
        }

        public async Task<bool> RegistrarNotaConocimientosAsync(int idPostulacion, decimal nota)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            // Cambiamos a ExecuteScalarAsync que lee de forma directa el SELECT @@ROWCOUNT del SP
            var filasAfectadas = await connection.ExecuteScalarAsync<int>(
                "sp_Comite_RegistrarNotaConocimientos",
                new
                {
                    IdPostulacion = idPostulacion,
                    NotaConocimientos = nota
                },
                commandType: CommandType.StoredProcedure
            );

            return filasAfectadas > 0;
        }

        public async Task<byte[]> ObtenerActaConocimientosPdfAsync(List<EvaluacionConocimientosDto> candidatos, string codigoConvocatoria, string nombrePuesto)
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9.5f).FontFamily("Arial"));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("SISTEMA DE CONVOCATORIAS DE PERSONAL (SCP)").FontSize(12).Bold().FontColor(Colors.Blue.Darken3);
                        column.Item().Text("ACTA DE RESULTADOS - EVALUACIÓN DE CONOCIMIENTOS").FontSize(10).Bold().FontColor(Colors.Grey.Darken2);
                        column.Item().PaddingTop(4).LineHorizontal(0.8f).LineColor(Colors.Grey.Lighten1);
                        column.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(cd => { cd.ConstantColumn(90); cd.RelativeColumn(); });
                            table.Cell().PaddingVertical(2).Text("Convocatoria:").Bold();
                            table.Cell().PaddingVertical(2).Text(codigoConvocatoria);
                            table.Cell().PaddingVertical(2).Text("Puesto CAS:").Bold();
                            table.Cell().PaddingVertical(2).Text(nombrePuesto);
                        });
                        column.Item().PaddingTop(12);
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(cd =>
                        {
                            cd.ConstantColumn(30);  // N°
                            cd.ConstantColumn(120); // Expediente
                            cd.RelativeColumn();    // Postulante
                            cd.ConstantColumn(70);  // Nota
                            cd.ConstantColumn(110); // Condición
                        });

                        table.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("N°").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("N° Expediente").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Apellidos y Nombres").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Nota").Bold().AlignCenter();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Condición").Bold();
                        });

                        int contador = 1;
                        foreach (var p in candidatos)
                        {
                            string condicionText = p.IdEstadoPostulacionCat == 1108 ? "PENDIENTE" :
                                                   p.IdEstadoPostulacionCat == 19 ? "APTO" : "NO APTO";

                            var colorCondicion = p.IdEstadoPostulacionCat == 19 ? Colors.Green.Darken2 :
                                                 p.IdEstadoPostulacionCat == 1109 ? Colors.Red.Darken2 : Colors.Orange.Darken2;

                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(contador.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.CodigoPostulacionUnid);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.PostulanteNombre);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.NotaConocimientos.ToString("F2")).AlignCenter();
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(condicionText).Bold().FontColor(colorCondicion);

                            contador++;
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(" de ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();

            return pdfBytes;
        }

        public async Task<List<CalificacionCurricularDto>> ListarCandidatosCurricularAsync(int idPlaza)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var resultado = await connection.QueryAsync<CalificacionCurricularDto>(
                "sp_Comite_ListarCandidatosCurricular",
                new { IdPlaza = idPlaza },
                commandType: CommandType.StoredProcedure
            );
            return resultado.ToList();
        }

        public async Task<bool> RegistrarCalificacionCurricularAsync(int idPostulacion, decimal notaFormacion, decimal notaCapacitacion, decimal notaExperiencia)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var filasAfectadas = await connection.ExecuteScalarAsync<int>(
                "sp_Comite_RegistrarCalificacionCurricular",
                new
                {
                    IdPostulacion = idPostulacion,
                    NotaFormacion = notaFormacion,
                    NotaCapacitacion = notaCapacitacion,
                    NotaExperiencia = notaExperiencia
                },
                commandType: CommandType.StoredProcedure
            );

            return filasAfectadas > 0;
        }

        public async Task<byte[]> ObtenerActaCurricularPdfAsync(List<CalificacionCurricularDto> candidatos, string codigoConvocatoria, string nombrePuesto)
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9f).FontFamily("Arial"));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("SISTEMA DE CONVOCATORIAS DE PERSONAL").FontSize(12).Bold().FontColor(Colors.Blue.Darken3);
                        column.Item().Text("ACTA DE EVALUACIÓN - ETAPA CALIFICACIÓN CURRICULAR").FontSize(10).Bold().FontColor(Colors.Grey.Darken2);
                        column.Item().PaddingTop(4).LineHorizontal(0.8f).LineColor(Colors.Grey.Lighten1);
                        column.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(cd => { cd.ConstantColumn(90); cd.RelativeColumn(); });
                            table.Cell().PaddingVertical(2).Text("Convocatoria:").Bold();
                            table.Cell().PaddingVertical(2).Text(codigoConvocatoria);
                            table.Cell().PaddingVertical(2).Text("Puesto CAS:").Bold();
                            table.Cell().PaddingVertical(2).Text(nombrePuesto);
                        });
                        column.Item().PaddingTop(12);
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(cd =>
                        {
                            cd.ConstantColumn(25);  // N°
                            cd.ConstantColumn(90);  // Expediente
                            cd.RelativeColumn();    // Postulante
                            cd.ConstantColumn(55);  // Ptje Acad.
                            cd.ConstantColumn(55);  // Ptje Cursos
                            cd.ConstantColumn(55);  // Ptje Lab.
                            cd.ConstantColumn(55);  // Final
                            cd.ConstantColumn(75);  // Condición
                        });

                        table.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("N°").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("N° Expediente").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Apellidos y Nombres").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Acad.").Bold().AlignCenter();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Cursos").Bold().AlignCenter();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Laboral").Bold().AlignCenter();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Final").Bold().AlignCenter();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Condición").Bold();
                        });

                        int contador = 1;
                        foreach (var p in candidatos)
                        {
                            string condicionText = p.IdEstadoPostulacionCat == 19 ? "PENDIENTE" :
                                                   p.IdEstadoPostulacionCat == 21 ? "APTO" : "NO APTO";

                            var colorCondicion = p.IdEstadoPostulacionCat == 21 ? Colors.Green.Darken2 :
                                                 p.IdEstadoPostulacionCat == 1109 ? Colors.Red.Darken2 : Colors.Orange.Darken2;

                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(contador.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.CodigoPostulacionUnid);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.PostulanteNombre);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.NotaFormacion.ToString("F2")).AlignCenter();
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.NotaCapacitacion.ToString("F2")).AlignCenter();
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.NotaExperiencia.ToString("F2")).AlignCenter();
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.NotaCurricularFinal.ToString("F2")).AlignCenter().Bold();
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(condicionText).Bold().FontColor(colorCondicion);

                            contador++;
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(" de ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();

            return pdfBytes;
        }
    }
}
