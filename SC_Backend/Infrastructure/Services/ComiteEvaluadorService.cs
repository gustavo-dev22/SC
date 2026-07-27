using Application.Admin.Dtos;
using Application.Comite.Dtos;
using Application.Common.Interfaces;
using Dapper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                                // 💡 NUEVA LÓGICA BASADA EN COLUMNAS DE AUDITORÍA FLUIDAS
                                string condicionText = "PENDIENTE";
                                var colorCondicion = Colors.Orange.Darken2;

                                if (p.FaseExpedientesAprobado == true)
                                {
                                    condicionText = "APTO";
                                    colorCondicion = Colors.Green.Darken2;
                                }
                                else if (p.FaseExpedientesAprobado == false)
                                {
                                    condicionText = "NO APTO";
                                    colorCondicion = Colors.Red.Darken2;
                                }

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
                            string condicionText = "PENDIENTE";
                            var colorCondicion = Colors.Orange.Darken2;

                            // Evaluamos la bandera estable
                            if (p.FaseConocimientosAprobado == true)
                            {
                                condicionText = "APTO";
                                colorCondicion = Colors.Green.Darken2;
                            }
                            else if (p.FaseConocimientosAprobado == false)
                            {
                                condicionText = "NO APTO";
                                colorCondicion = Colors.Red.Darken2;
                            }

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
                            string condicionText = "PENDIENTE";
                            var colorCondicion = Colors.Orange.Darken2;

                            if (p.FaseCurricularAprobado == true)
                            {
                                condicionText = "APTO";
                                colorCondicion = Colors.Green.Darken2;
                            }
                            else if (p.FaseCurricularAprobado == false)
                            {
                                condicionText = "NO APTO";
                                colorCondicion = Colors.Red.Darken2;
                            }

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

        public async Task<List<EvaluacionEntrevistaDto>> ListarCandidatosEntrevistaAsync(int idPlaza)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var resultado = await connection.QueryAsync<EvaluacionEntrevistaDto>(
                "sp_Comite_ListarCandidatosEntrevista",
                new { IdPlaza = idPlaza },
                commandType: CommandType.StoredProcedure
            );
            return resultado.ToList();
        }

        public async Task<bool> RegistrarNotaEntrevistaAsync(int idPostulacion, decimal nota)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var filasAfectadas = await connection.ExecuteScalarAsync<int>(
                "sp_Comite_RegistrarNotaEntrevista",
                new { IdPostulacion = idPostulacion, NotaEntrevista = nota },
                commandType: CommandType.StoredProcedure
            );
            return filasAfectadas > 0;
        }

        public async Task<byte[]> ObtenerActaEntrevistaPdfAsync(List<EvaluacionEntrevistaDto> candidatos, string codigoConvocatoria, string nombrePuesto)
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9.5f).FontFamily("Arial"));

                    // ENCABEZADO DEL ACTA FINAL
                    page.Header().Column(column =>
                    {
                        column.Item().Text("SISTEMA DE CONVOCATORIAS DE PERSONAL (SCP)").FontSize(12).Bold().FontColor(Colors.Blue.Darken3);
                        column.Item().Text("ACTA DE RESULTADOS FINALES - EVALUACIÓN DE ENTREVISTA PERSONAL").FontSize(10).Bold().FontColor(Colors.Grey.Darken2);
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

                    // TABLA DE POSTULANTES CON NOTA DE ENTREVISTA
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(cd =>
                        {
                            cd.ConstantColumn(30);  // N°
                            cd.ConstantColumn(120); // Expediente
                            cd.RelativeColumn();    // Postulante
                            cd.ConstantColumn(80);  // Nota Entrevista
                            cd.ConstantColumn(110); // Condición Final
                        });

                        table.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("N°").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("N° Expediente").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Apellidos y Nombres").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Nota").Bold().AlignCenter();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Resultado Final").Bold();
                        });

                        int contador = 1;
                        foreach (var p in candidatos)
                        {
                            string condicionText = "PENDIENTE";
                            var colorCondicion = Colors.Orange.Darken2;

                            if (p.FaseEntrevistaAprobado == true)
                            {
                                condicionText = "APTO";
                                colorCondicion = Colors.Green.Darken2;
                            }
                            else if (p.FaseEntrevistaAprobado == false)
                            {
                                condicionText = "NO APTO";
                                colorCondicion = Colors.Red.Darken2;
                            }

                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(contador.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.CodigoPostulacionUnid);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.PostulanteNombre);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.NotaEntrevista.ToString("F2")).AlignCenter();
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(condicionText).Bold().FontColor(colorCondicion);

                            contador++;
                        }
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

        public async Task<List<CuadroMeritoFinalDto>> ObtenerCuadroMeritoFinalAsync(int idPlaza)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var resultado = await connection.QueryAsync<CuadroMeritoFinalDto>(
                "sp_Comite_ListarCuadroMeritoFinal",
                new { IdPlaza = idPlaza },
                commandType: CommandType.StoredProcedure
            );
            return resultado.ToList();
        }

        public async Task<byte[]> ObtenerActaFinalConsolidadaPdfAsync(List<CuadroMeritoFinalDto> candidatos, string codigoConvocatoria, string nombrePuesto)
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0.8f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(7.5f).FontFamily("Arial"));

                    // ENCABEZADO OFICIAL
                    page.Header().Column(column =>
                    {
                        column.Item().Text("SISTEMA DE CONVOCATORIAS DE PERSONAL (SCP)").FontSize(11).Bold().FontColor(Colors.Blue.Darken3);
                        column.Item().Text("CUADRO CONSOLIDADO DE MÉRITOS - RESULTADOS FINALES").FontSize(9.5f).Bold().FontColor(Colors.Grey.Darken2);
                        column.Item().PaddingTop(3).LineHorizontal(0.8f).LineColor(Colors.Grey.Lighten1);
                        column.Item().PaddingTop(4).Table(table =>
                        {
                            table.ColumnsDefinition(cd => { cd.ConstantColumn(85); cd.RelativeColumn(); });
                            table.Cell().Text("Convocatoria:").Bold(); table.Cell().Text(codigoConvocatoria);
                            table.Cell().Text("Puesto CAS:").Bold(); table.Cell().Text(nombrePuesto);
                        });
                        column.Item().PaddingTop(6);
                    });

                    // TABLA MATRIZ CON BONIFICACIONES
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(cd =>
                        {
                            cd.ConstantColumn(20);  // N°
                            cd.ConstantColumn(75);  // Expediente
                            cd.RelativeColumn();    // Postulante
                            cd.ConstantColumn(30);  // F1 (Exp)
                            cd.ConstantColumn(32);  // EC (40%)
                            cd.ConstantColumn(32);  // CV (30%)
                            cd.ConstantColumn(32);  // ET (30%)
                            cd.ConstantColumn(40);  // Ptje. Base
                            cd.ConstantColumn(38);  // Bonif. CONADIS
                            cd.ConstantColumn(38);  // Bonif. FFAA
                            cd.ConstantColumn(38);  // Bonif. DECAN
                            cd.ConstantColumn(42);  // Prom. Final
                            cd.ConstantColumn(75);  // Situación
                        });

                        table.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Darken3).Padding(3).Text("N°").Bold().FontColor(Colors.White).AlignCenter();
                            h.Cell().Background(Colors.Grey.Darken3).Padding(3).Text("Expediente").Bold().FontColor(Colors.White);
                            h.Cell().Background(Colors.Grey.Darken3).Padding(3).Text("Apellidos y Nombres").Bold().FontColor(Colors.White);
                            h.Cell().Background(Colors.Grey.Darken3).Padding(3).Text("EXP").Bold().FontColor(Colors.White).AlignCenter();
                            h.Cell().Background(Colors.Grey.Darken3).Padding(3).Text("EC").Bold().FontColor(Colors.White).AlignCenter();
                            h.Cell().Background(Colors.Grey.Darken3).Padding(3).Text("CV").Bold().FontColor(Colors.White).AlignCenter();
                            h.Cell().Background(Colors.Grey.Darken3).Padding(3).Text("ET").Bold().FontColor(Colors.White).AlignCenter();
                            h.Cell().Background(Colors.Grey.Darken2).Padding(3).Text("P. Base").Bold().FontColor(Colors.White).AlignCenter();

                            // Columnas de Bonificación
                            h.Cell().Background(Colors.Amber.Darken3).Padding(3).Text("CONADIS").Bold().FontColor(Colors.White).AlignCenter();
                            h.Cell().Background(Colors.Amber.Darken3).Padding(3).Text("FF.AA.").Bold().FontColor(Colors.White).AlignCenter();
                            h.Cell().Background(Colors.Amber.Darken3).Padding(3).Text("DECAN").Bold().FontColor(Colors.White).AlignCenter();

                            h.Cell().Background(Colors.Blue.Darken4).Padding(3).Text("Final").Bold().FontColor(Colors.White).AlignCenter();
                            h.Cell().Background(Colors.Grey.Darken3).Padding(3).Text("Situación").Bold().FontColor(Colors.White);
                        });

                        int contador = 1;
                        foreach (var p in candidatos)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(contador.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(p.CodigoPostulacionUnid);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(p.PostulanteNombre).Bold();

                            // Fases eliminatorias
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(p.FaseExpedientesAprobado == true ? "APTO" : "NO APTO");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(p.PtjePonderadoConocimientos?.ToString("F2") ?? "---");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(p.PtjePonderadoCurricular?.ToString("F2") ?? "---");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(p.PtjePonderadoEntrevista?.ToString("F2") ?? "---");

                            // Puntaje Base acumulado
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(p.PtjeBaseAcumulado.ToString("F2")).Bold();

                            // Bonificaciones
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(p.PtjeBonifConadis > 0 ? $"+{p.PtjeBonifConadis:F2}" : "---");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(p.PtjeBonifFFAA > 0 ? $"+{p.PtjeBonifFFAA:F2}" : "---");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(p.PtjeBonifDecan > 0 ? $"+{p.PtjeBonifDecan:F2}" : "---");

                            // Nota Final y Estado
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(p.NotaFinalAcumulada?.ToString("F2") ?? "---").Bold();
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(p.SituacionFinalDesc).Bold();

                            contador++;
                        }
                    });

                    // PIE DE PÁGINA CON FECHA Y PAGINACIÓN
                    page.Footer().Row(row =>
                    {
                        row.RelativeItem().Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(7).FontColor(Colors.Grey.Darken1);
                        row.RelativeItem().AlignRight().Text(x =>
                        {
                            x.Span("Página ").FontSize(7);
                            x.CurrentPageNumber().FontSize(7);
                            x.Span(" de ").FontSize(7);
                            x.TotalPages().FontSize(7);
                        });
                    });
                });
            }).GeneratePdf();

            return pdfBytes;
        }

        public async Task<List<AdminTicketBandejaDto>> ObtenerConsultasTecnicasAsync(int? idEstado, string? busqueda)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var result = await connection.QueryAsync<AdminTicketBandejaDto>(
                "sp_Comite_SoporteTicket_ListarConsultasTecnicas",
                new { IdEstadoTicketCat = idEstado, Busqueda = busqueda },
                commandType: CommandType.StoredProcedure
            );
            return result.ToList();
        }

        public async Task<ComiteDashboardDto> ObtenerDashboardComiteAsync(string nombreUsuario)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                "sp_Comite_Dashboard_ObtenerResumen",
                new { NombreUsuario = nombreUsuario },
                commandType: CommandType.StoredProcedure
            );

            var dto = new ComiteDashboardDto();
            dto.Metricas = await multi.ReadFirstAsync<ComiteMetricasDto>();
            dto.DistribucionEstados = (await multi.ReadAsync<DistribucionEstadosDto>()).ToList();

            return dto;
        }

        public async Task<bool> DeclararPlazaDesiertaAsync(int idPlaza, int idMotivoDesiertaCat, string sustentoDesierta, string usuarioDeclara)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var parametros = new
            {
                IdPlaza = idPlaza,
                IdMotivoDesiertaCat = idMotivoDesiertaCat,
                SustentoDesierta = sustentoDesierta,
                UsuarioDeclara = usuarioDeclara
            };

            // Leemos el resultado (1 = Exito, 0 = Fallo)
            var resultado = await connection.QueryFirstOrDefaultAsync<int>(
                "sp_Comite_DeclararPlazaDesierta",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return resultado == 1;
        }

        public async Task<byte[]> ObtenerActaDesiertaPdfAsync(int idPlaza)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            // 🚀 1. Leemos los datos de la plaza y la observación registrada al declarar desierta
            var infoPlaza = await connection.QueryFirstOrDefaultAsync<dynamic>(
                @"SELECT TOP 1 
            p.codigo_postulacion_unid AS CodigoConvocatoria,
            p.observaciones_comite AS Observaciones,
            pos.nombres AS NombrePuesto -- O el campo de puesto correspondiente
          FROM sc_postulacion p
          INNER JOIN sc_postulante pos ON p.id_postulante = pos.id_postulante
          WHERE p.id_plaza = @IdPlaza AND p.activo = 1",
                new { IdPlaza = idPlaza }
            );

            string codigoConvocatoria = infoPlaza?.CodigoConvocatoria ?? $"PLAZA-{idPlaza}";
            string nombrePuesto = infoPlaza?.NombrePuesto ?? "PUESTO CAS";
            string observacionesText = infoPlaza?.Observaciones ?? "DECLARADO DESIERTO POR EL COMITÉ EVALUADOR";

            // 🚀 2. Construimos el documento PDF con QuestPDF
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.8f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9.5f).FontFamily("Arial"));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("SISTEMA DE CONVOCATORIAS DE PERSONAL (SCP)").FontSize(11).Bold().FontColor(Colors.Blue.Darken3).AlignCenter();
                        column.Item().Text("COMITÉ EVALUADOR DE SELECCIÓN DE PERSONAL").FontSize(9.5f).Bold().FontColor(Colors.Grey.Darken2).AlignCenter();
                        column.Item().PaddingTop(6).LineHorizontal(0.8f).LineColor(Colors.Grey.Darken1);
                    });

                    page.Content().PaddingTop(12).Column(col =>
                    {
                        col.Item().Text("ACTA DE DECLARACIÓN DE PLAZA DESIERTA").FontSize(13).Bold().FontColor(Colors.Red.Darken3).AlignCenter();

                        col.Item().PaddingTop(12).Text($"En la ciudad de Lima, a los {DateTime.Now:dd} días del mes de {DateTime.Now:MMMM} del año {DateTime.Now:yyyy}, el Comité Evaluador asignado a la conducción del proceso de selección de personal, da fe de lo siguiente:").Justify();

                        col.Item().PaddingTop(12).Background(Colors.Grey.Lighten4).Padding(8).Column(info =>
                        {
                            info.Item().Text($"• Convocatoria / Código: {codigoConvocatoria}").Bold();
                            info.Item().Text($"• ID de Plaza Evaluada: {idPlaza}").Bold();
                            info.Item().Text($"• Fecha de Cierre Oficial: {DateTime.Now:dd/MM/yyyy HH:mm} horas").Bold();
                        });

                        col.Item().PaddingTop(12).Text("I. ANTECEDENTES Y CONSIDERANDOS").Bold().FontSize(10.5f);
                        col.Item().PaddingTop(4).Text("Habiéndose culminado las etapas evaluativas correspondientes al cronograma oficial establecido en las bases del proceso, y tras la revisión del cuadro consolidado de méritos, se verificó que no se cuenta con postulantes aptos en condición de ganador para la cobertura de la vacante.").Justify();

                        col.Item().PaddingTop(12).Text("II. DETERMINACIÓN DEL COMITÉ").Bold().FontSize(10.5f);
                        col.Item().PaddingTop(4).Text("Por intermedio del presente acto, el Comité Evaluador acuerda declarar la plaza en condición de DESIERTA en atención al siguiente registro oficial:").Justify();

                        col.Item().PaddingTop(8).Border(1).BorderColor(Colors.Grey.Darken1).Padding(8).Column(motivoBox =>
                        {
                            motivoBox.Item().Text("ACUERDO DEL COMITÉ EVALUADOR:").Bold().FontColor(Colors.Red.Darken2);
                            motivoBox.Item().PaddingTop(4).Text(observacionesText).Italic();
                        });

                        col.Item().PaddingTop(12).Text("En fe de lo cual, los miembros del Comité Evaluador firman el presente documento para su incorporación al expediente administrativo y posterior publicación.").Justify();

                        // SECCIÓN DE FIRMAS
                        col.Item().PaddingTop(50).Row(row =>
                        {
                            row.RelativeItem().Column(f1 =>
                            {
                                f1.Item().LineHorizontal(0.8f).LineColor(Colors.Grey.Darken2);
                                f1.Item().PaddingTop(3).Text("PRESIDENTE DEL COMITÉ").Bold().AlignCenter();
                                f1.Item().Text("Comité Evaluador").FontSize(8).AlignCenter();
                            });
                            row.ConstantItem(25);
                            row.RelativeItem().Column(f2 =>
                            {
                                f2.Item().LineHorizontal(0.8f).LineColor(Colors.Grey.Darken2);
                                f2.Item().PaddingTop(3).Text("SECRETARIO TÉCNICO").Bold().AlignCenter();
                                f2.Item().Text("Comité Evaluador").FontSize(8).AlignCenter();
                            });
                            row.ConstantItem(25);
                            row.RelativeItem().Column(f3 =>
                            {
                                f3.Item().LineHorizontal(0.8f).LineColor(Colors.Grey.Darken2);
                                f3.Item().PaddingTop(3).Text("INTEGRANTE ÁREA USUARIA").Bold().AlignCenter();
                                f3.Item().Text("Comité Evaluador").FontSize(8).AlignCenter();
                            });
                        });
                    });

                    page.Footer().AlignRight().Text($"Página 1 de 1 - Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                });
            }).GeneratePdf();

            return pdfBytes;
        }
    }
}
