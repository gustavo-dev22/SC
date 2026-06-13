using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Postulantes.Dtos;
using Application.Postulantes.Dtos.FichaPostulante;
using Dapper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Services
{
    public class PostulanteQueryService : IPostulanteQueryService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public PostulanteQueryService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<dynamic> ObtenerByDocumentoAsync(string numDocumento)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Postulante_ObtenerByDocumento",
                new { NumDocumento = numDocumento },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<PerfilPostulanteDto?> ObtenerPerfilByIdAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PerfilPostulanteDto>(
                "sp_Postulante_ObtenerPerfil",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<List<PostulanteFormacionDto>> ListarFormacionAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteFormacionDto>(
                "sp_PostulanteFormacion_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<PostulanteCertificacionDto>> ListarCertificacionesAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteCertificacionDto>(
                "sp_PostulanteCertificacion_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<PostulanteExperienciaDto>> ListarExperienciaAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteExperienciaDto>(
                "sp_PostulanteExperiencia_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<bool> ExisteSuperposicionLaboralAsync(int idPostulante, int idExperiencia, DateTime fechaInicio, DateTime? fechaFin)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var parametros = new
            {
                IdPostulante = idPostulante,
                IdExperiencia = idExperiencia,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin 
            };

            int coincidencias = await connection.ExecuteScalarAsync<int>(
                "sp_PostulanteExperiencia_ValidarSuperposicion",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return coincidencias > 0;
        }

        public async Task<List<PostulanteColegiaturaDto>> ListarColegiaturasAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteColegiaturaDto>(
                "sp_PostulanteColegiatura_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<PostulanteIdiomaDto>> ListarIdiomasAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteIdiomaDto>(
                "sp_PostulanteIdioma_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<PostulanteOfimaticaDto>> ListarOfimaticaAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteOfimaticaDto>(
                "sp_PostulanteOfimatica_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<PostulanteRequisitoEspecialDto>> ListarRequisitosEspecialesAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteRequisitoEspecialDto>(
                "sp_PostulanteRequisitoEspecial_Listar",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<AvanceCurriculumDto> ObtenerAvanceCurriculumAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AvanceCurriculumDto>(
                "sp_Postulante_ObtenerAvanceCurriculum",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            ) ?? new AvanceCurriculumDto();
        }

        public async Task<List<UbigeoDto>> ObtenerDepartamentosAsync()
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            const string sql = "SELECT id_departamento AS Id, nombre AS Nombre FROM sc_ubigeo_departamento WHERE activo = 1 ORDER BY nombre";
            return (await connection.QueryAsync<UbigeoDto>(sql)).ToList();
        }

        public async Task<List<UbigeoDto>> ObtenerProvinciasAsync(string idDepartamento)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            const string sql = "SELECT id_provincia AS Id, nombre AS Nombre FROM sc_ubigeo_provincia WHERE id_departamento = @IdDep AND activo = 1 ORDER BY nombre";
            return (await connection.QueryAsync<UbigeoDto>(sql, new { IdDep = idDepartamento })).ToList();
        }

        public async Task<List<UbigeoDto>> ObtenerDistritosAsync(string idProvincia)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            const string sql = "SELECT id_distrito AS Id, nombre AS Nombre FROM sc_ubigeo_distrito WHERE id_provincia = @IdProv AND activo = 1 ORDER BY nombre";
            return (await connection.QueryAsync<UbigeoDto>(sql, new { IdProv = idProvincia })).ToList();
        }

        public async Task<InfoAdicionalDto?> ObtenerInfoAdicionalAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "sp_Postulante_ObtenerInfoAdicional",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            );

            var info = await multi.ReadFirstOrDefaultAsync<dynamic>();
            if (info == null) return new InfoAdicionalDto(idPostulante, false, new List<string>());

            var deptsIds = (await multi.ReadAsync<string>()).ToList();

            return new InfoAdicionalDto(
                (int)info.IdPostulante,
                (bool)info.DisponibilidadInterior,
                deptsIds
            );
        }

        public async Task<PostulanteFirmaDto?> ObtenerFirmaAsync(int idPostulante)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var resultado = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Postulante_ObtenerFirma",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            );

            if (resultado == null || resultado.FirmaDigitalizada == null)
                return new PostulanteFirmaDto(idPostulante, null, null);

            byte[] bytes = (byte[])resultado.FirmaDigitalizada;
            string base64String = Convert.ToBase64String(bytes);
            string tipoMime = resultado.FirmaTipoMime;

            return new PostulanteFirmaDto(
                idPostulante,
                $"data:{tipoMime};base64,{base64String}",
                tipoMime
            );
        }

        public async Task<byte[]> ObtenerFichaPdfAsync(int idPostulante)
        {
            var ficha = new FichaReporteDto();

            // 🚀 BLINDADO: La conexión se abre de forma segura
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            // 🚀 BLINDADO: El lector múltiple se cierra inmediatamente al terminar de leer, liberando SQL Server al 100%
            using var multi = await connection.QueryMultipleAsync(
                "sp_Postulante_ObtenerFichaReporte",
                new { IdPostulante = idPostulante },
                commandType: CommandType.StoredProcedure
            );

            // Mapeo secuencial estricto en el mismo orden del SP
            ficha.Cabecera = await multi.ReadFirstOrDefaultAsync<CabeceraPostulanteReporte>() ?? new();
            ficha.Formaciones = (await multi.ReadAsync<FormacionReporte>()).ToList();
            ficha.Colegiaturas = (await multi.ReadAsync<ColegiaturaReporte>()).ToList();
            ficha.Idiomas = (await multi.ReadAsync<IdiomaReporte>()).ToList();
            ficha.Ofimaticas = (await multi.ReadAsync<OfimaticaReporte>()).ToList();
            ficha.Certificaciones = (await multi.ReadAsync<CertificacionReporte>()).ToList();
            ficha.Experiencias = (await multi.ReadAsync<ExperienciaReporte>()).ToList();
            ficha.OtrosRequisitos = (await multi.ReadAsync<OtrosRequisitosReporte>()).ToList();
            ficha.InfoAdicional = await multi.ReadFirstOrDefaultAsync<InfoAdicionalReporte>() ?? new();
            ficha.FirmaBytes = await multi.ReadFirstOrDefaultAsync<byte[]>();

            // Aquí la base de datos ya está completamente liberada y cerrada. QuestPDF procesa en memoria pura.
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.2f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    // CABECERA GENERAL
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("SISTEMA DE CONVOCATORIAS DE PERSONAL (SCP)").FontSize(12).Bold().FontColor(Colors.Blue.Darken3);
                            col.Item().Text("FICHA RESUMEN DE DECLARACIÓN JURADA DEL POSTULANTE").FontSize(9).Bold().FontColor(Colors.Grey.Darken2);
                        });
                        row.ConstantItem(80).Text($"ID-{ficha.Cabecera.IdPostulante:D6}").FontSize(10).Bold().AlignRight();
                    });

                    page.Content().PaddingVertical(0.5f, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(18);

                        // A) DATOS PERSONALES
                        col.Item().Column(seccion =>
                        {
                            seccion.Item().Text("1. DATOS PERSONALES").FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken3);
                            seccion.Item().PaddingTop(3).Table(table =>
                            {
                                table.ColumnsDefinition(cd => { cd.ConstantColumn(110); cd.RelativeColumn(); cd.ConstantColumn(90); cd.RelativeColumn(); });

                                table.Cell().PaddingVertical(3).Text("Nombres y Apellidos:").Bold();
                                table.Cell().ColumnSpan(3).PaddingVertical(3).Text($"{ficha.Cabecera.ApellidoPaterno} {ficha.Cabecera.ApellidoMaterno}, {ficha.Cabecera.Nombres}");

                                table.Cell().PaddingVertical(3).Text("DNI / Documento:").Bold();
                                table.Cell().PaddingVertical(3).Text(ficha.Cabecera.NumDocumento);

                                table.Cell().PaddingVertical(3).Text("Sexo:").Bold();
                                table.Cell().PaddingVertical(3).Text(ficha.Cabecera.Sexo);

                                table.Cell().PaddingVertical(3).Text("Correo Electrónico:").Bold();
                                table.Cell().PaddingVertical(3).Text(ficha.Cabecera.Correo);

                                table.Cell().PaddingVertical(3).Text("Teléfono Celular:").Bold();
                                table.Cell().PaddingVertical(3).Text(ficha.Cabecera.Telefono);

                                table.Cell().PaddingVertical(3).Text("Dirección Declarada:").Bold();
                                table.Cell().ColumnSpan(3).PaddingVertical(3).Text($"{ficha.Cabecera.Direccion} (Ref: {ficha.Cabecera.ReferenciaDireccion})");

                                table.Cell().PaddingVertical(3).Text("Ubigeo Geográfico:").Bold();
                                table.Cell().ColumnSpan(3).PaddingVertical(3).Text(ficha.Cabecera.UbigeoCompleto);
                            });
                        });

                        // B) FORMACIÓN ACADÉMICA
                        col.Item().Column(seccion =>
                        {
                            seccion.Item().Text("2. FORMACIÓN ACADÉMICA").FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken3);
                            seccion.Item().PaddingTop(3).Table(table =>
                            {
                                table.ColumnsDefinition(cd => { cd.ConstantColumn(100); cd.RelativeColumn(); cd.RelativeColumn(); cd.ConstantColumn(70); cd.ConstantColumn(70); });

                                table.Header(h => {
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Nivel").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Centro de Estudios").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Carrera / Especialidad").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("F. Término").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Condición").Bold();
                                });

                                foreach (var f in ficha.Formaciones)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(f.NivelEstudio);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(f.CentroEstudios);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(f.CarreraEspecialidad);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(f.PeriodoFin);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(f.EstadoEstudio);
                                }
                            });
                        });

                        // C) COLEGIATURA
                        col.Item().Column(seccion =>
                        {
                            seccion.Item().Text("3. COLEGIATURAS Y REGISTROS PROFESIONALES").FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken3);
                            seccion.Item().PaddingTop(3).Table(table =>
                            {
                                table.ColumnsDefinition(cd => { cd.RelativeColumn(); cd.ConstantColumn(100); cd.ConstantColumn(90); cd.ConstantColumn(80); });

                                table.Header(h => {
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Colegio Profesional").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("N° Registro").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Fecha Inc.").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("¿Habilitado?").Bold();
                                });

                                foreach (var c in ficha.Colegiaturas)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(c.ColegioProfesional);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(c.NumeroColegiacion);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(c.FechaColegiacion?.ToString("dd/MM/yyyy") ?? "-");
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(c.CondicionHabilitado);
                                }
                            });
                        });

                        // D) IDIOMAS Y E) OFIMÁTICA
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().PaddingRight(6).Column(iCol => {
                                iCol.Item().Text("4. IDIOMAS / DIALECTOS").FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken3);
                                iCol.Item().PaddingTop(3).Table(table => {
                                    table.ColumnsDefinition(cd => { cd.RelativeColumn(); cd.RelativeColumn(); });
                                    table.Header(h => {
                                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Idioma").Bold();
                                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Nivel").Bold();
                                    });
                                    foreach (var idm in ficha.Idiomas)
                                    {
                                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(idm.Idioma);
                                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(idm.NivelConocimiento);
                                    }
                                });
                            });

                            row.RelativeItem().PaddingLeft(6).Column(oCol => {
                                oCol.Item().Text("5. CONOCIMIENTOS DE OFIMÁTICA").FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken3);
                                oCol.Item().PaddingTop(3).Table(table => {
                                    table.ColumnsDefinition(cd => { cd.RelativeColumn(); cd.RelativeColumn(); });
                                    table.Header(h => {
                                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Herramienta").Bold();
                                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Nivel").Bold();
                                    });
                                    foreach (var ofi in ficha.Ofimaticas)
                                    {
                                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(ofi.HerramientaOfimática);
                                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(ofi.NivelConocimiento);
                                    }
                                });
                            });
                        });

                        // F) CERTIFICACIONES Y/O ESPECIALIZACIONES
                        col.Item().Column(seccion =>
                        {
                            seccion.Item().Text("6. CERTIFICACIONES Y/O ESPECIALIZACIONES").FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken3);
                            seccion.Item().PaddingTop(3).Table(table =>
                            {
                                table.ColumnsDefinition(cd => { cd.ConstantColumn(120); cd.RelativeColumn(); cd.RelativeColumn(); cd.ConstantColumn(50); cd.ConstantColumn(70); });
                                table.Header(h => {
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Tipo").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Nombre del Programa").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Institución Emisora").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Horas").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Fecha").Bold();
                                });
                                foreach (var cert in ficha.Certificaciones)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(cert.Tipo);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(cert.NombreCurso);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(cert.InstitucionEmisora);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(cert.HorasLectivas.ToString());
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(cert.FechaCertificacion?.ToString("dd/MM/yyyy") ?? "-");
                                }
                            });
                        });

                        // G) EXPERIENCIA LABORAL
                        col.Item().Column(seccion =>
                        {
                            seccion.Item().Text("7. EXPERIENCIA LABORAL DECLARADA").FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken3);
                            seccion.Item().PaddingTop(3).Table(table =>
                            {
                                table.ColumnsDefinition(cd => { cd.RelativeColumn(2); cd.RelativeColumn(2); cd.ConstantColumn(60); cd.ConstantColumn(60); cd.ConstantColumn(70); cd.ConstantColumn(55); });
                                table.Header(h => {
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Institución / Empresa").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Cargo Desempeñado").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("F. Inicio").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("F. Término").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Régimen").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Remun.").Bold();
                                });
                                foreach (var e in ficha.Experiencias)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(e.EmpresaInstitucion);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(e.CargoPuesto);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(e.FechaInicio.ToString("dd/MM/yyyy"));
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(e.FechaFin?.ToString("dd/MM/yyyy") ?? "ACTUALIDAD");
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(e.Regimen);
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text($"S/. {e.Remuneracion:N2}");
                                }
                            });
                        });

                        // H) OTROS REQUISITOS
                        col.Item().Column(seccion =>
                        {
                            seccion.Item().Text("8. OTROS REQUISITOS O COMPROMISOS INSTITUCIONALES").FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken3);
                            seccion.Item().PaddingTop(3).Table(table =>
                            {
                                table.ColumnsDefinition(cd => { cd.ConstantColumn(180); cd.RelativeColumn(); });
                                foreach (var oReq in ficha.OtrosRequisitos)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(oReq.RequisitoClave).Bold();
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).Text(oReq.DetalleDeclarado);
                                }
                            });
                        });

                        // I) INFORMACIÓN ADICIONAL
                        col.Item().Column(seccion =>
                        {
                            seccion.Item().Text("9. INFORMACIÓN ADICIONAL").FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken3);
                            seccion.Item().PaddingTop(3).Table(table =>
                            {
                                table.ColumnsDefinition(cd => { cd.ConstantColumn(220); cd.RelativeColumn(); });
                                table.Cell().PaddingVertical(4).Text("¿Disponibilidad para trabajar en el interior del país?:").Bold();
                                table.Cell().PaddingVertical(4).Text(ficha.InfoAdicional.DisponibilidadInterior);
                                table.Cell().PaddingVertical(4).Text("Departamentos declarados disponibles:").Bold();
                                table.Cell().PaddingVertical(4).Text(ficha.InfoAdicional.DepartamentosDisponibles);
                            });
                        });

                        // J) FIRMA DIGITALIZADA
                        col.Item().AlignCenter().Width(180).Column(fCol =>
                        {
                            if (ficha.FirmaBytes != null && ficha.FirmaBytes.Length > 0)
                            {
                                fCol.Item().AlignCenter().Width(120).Image(ficha.FirmaBytes);
                            }
                            else
                            {
                                fCol.Item().PaddingTop(10).PaddingBottom(10).AlignCenter().Text("[ FALTA CARGAR FIRMA ]").FontColor(Colors.Red.Medium).Bold();
                            }

                            fCol.Item().PaddingTop(6);
                            // 🚀 CORREGIDO: Se cambiaron los .AlignCenter() finales del texto por .CenterAlign() nativo de QuestPDF
                            fCol.Item().BorderTop(1f).BorderColor(Colors.Grey.Darken1).AlignCenter().PaddingTop(2)
                                .Text("Firma Digitalizada")
                                .FontSize(8.5f).Bold().AlignCenter();

                            fCol.Item().AlignCenter().PaddingTop(2)
                                .Text($"{ficha.Cabecera.Nombres} {ficha.Cabecera.ApellidoPaterno} {ficha.Cabecera.ApellidoMaterno}")
                                .FontSize(9).AlignCenter();

                            fCol.Item().AlignCenter().PaddingTop(1)
                                .Text($"DNI: {ficha.Cabecera.NumDocumento}")
                                .FontSize(8.5f).FontColor(Colors.Grey.Darken3).AlignCenter();
                        });
                    });

                    // PIE DE PÁGINA
                    page.Footer().AlignCenter().Text(x => {
                        x.Span("Página ").FontSize(7.5f).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontSize(7.5f).FontColor(Colors.Grey.Medium);
                        x.Span(" de ").FontSize(7.5f).FontColor(Colors.Grey.Medium);
                        x.TotalPages().FontSize(7.5f).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();

            return pdfBytes;
        }

        public async Task<List<PostulanteDeclaracionDto>> ListarDeclaracionesAsync(int idPostulante, int idTipo)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            return (await connection.QueryAsync<PostulanteDeclaracionDto>(
                "sp_PostulanteDeclaracion_Listar",
                new { IdPostulante = idPostulante, IdTipoDeclaraciones = idTipo },
                commandType: CommandType.StoredProcedure
            )).ToList();
        }
    }
}
