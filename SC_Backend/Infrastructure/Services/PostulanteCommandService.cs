using Application.Auth.Dtos;
using Application.Common.Interfaces;
using Application.Postulantes.Commands;
using Application.Postulantes.Dtos;
using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PostulanteCommandService : IPostulanteCommandService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public PostulanteCommandService(IDbConnectionFactory dbFactory) => _dbConnectionFactory = dbFactory;

        public async Task<bool> RegistrarPostulanteAsync(RegistrarPostulanteCommand command, string computedHash)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@NumDocumento", command.NumDocumento);
            parameters.Add("@Nombres", command.Nombres);
            parameters.Add("@ApellidoPaterno", command.ApellidoPaterno);
            parameters.Add("@ApellidoMaterno", command.ApellidoMaterno);
            parameters.Add("@Correo", command.Correo);
            parameters.Add("@PasswordHash", computedHash);
            parameters.Add("@IdPostulante", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("sp_Postulante_Registrar", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<bool> ActualizarPerfilAsync(ActualizarPerfilCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var parametros = new
            {
                IdPostulante = command.IdPostulante,
                Telefono = command.Telefono,
                FechaNacimiento = command.FechaNacimiento,
                IdSexoCat = command.IdSexoCat,
                Direccion = command.Direccion,

                // Campos obligatorios ampliados
                IdTipoViaCat = command.IdTipoViaCat,
                IdTipoZonaCat = command.IdTipoZonaCat,
                NombreZona = command.NombreZona,
                IdUbigeoDistrito = command.IdUbigeoDistrito,

                NumeroVia = command.NumeroVia ?? string.Empty,
                NumeroDepto = command.NumeroDepto ?? string.Empty,
                Interior = command.Interior ?? string.Empty,
                Manzana = command.Manzana ?? string.Empty,
                Lote = command.Lote ?? string.Empty,
                Kilometro = command.Kilometro ?? string.Empty,
                BlockEdificio = command.BlockEdificio ?? string.Empty,
                Etapa = command.Etapa ?? string.Empty,
                ReferenciaDireccion = command.ReferenciaDireccion ?? string.Empty
            };

            await connection.ExecuteAsync(
                "sp_Postulante_ActualizarPerfil",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }

        public async Task<bool> MantenimientoFormacionAsync(MantenimientoFormacionCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            await connection.ExecuteAsync("sp_PostulanteFormacion_Mantenimiento", command, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<bool> MantenimientoCertificacionAsync(MantenimientoCertificacionCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            await connection.ExecuteAsync("sp_PostulanteCertificacion_Mantenimiento", command, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<bool> MantenimientoExperienciaAsync(MantenimientoExperienciaCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            // Blindamos las fechas por desborde y mapeamos el objeto para Dapper
            var parametros = new
            {
                Accion = command.Accion,
                IdExperiencia = command.IdExperiencia,
                IdPostulante = command.IdPostulante,
                EmpresaInstitucion = command.EmpresaInstitucion,
                CargoPuesto = command.CargoPuesto,
                FechaInicio = command.FechaInicio == default ? DateTime.Now : command.FechaInicio,
                FechaFin = command.FechaFin, // Puede ser null de forma nativa
                EsSectorPublico = command.EsSectorPublico,
                EsExperienciaEspecifica = command.EsExperienciaEspecifica,
                FuncionesPrincipales = command.FuncionesPrincipales,
                IdSectorCat = command.IdSectorCat,
                IdRegimenCat = command.IdRegimenCat,
                IdMotivoCambioCat = command.IdMotivoCambioCat,
                RemuneracionMensual = command.RemuneracionMensual
            };

            await connection.ExecuteAsync("sp_PostulanteExperiencia_Mantenimiento", parametros, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<bool> MantenimientoColegiaturaAsync(MantenimientoColegiaturaCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var parametros = new
            {
                Accion = command.Accion,
                IdColegiatura = command.IdColegiatura,
                IdPostulante = command.IdPostulante,
                IdColegioCat = command.IdColegioCat,
                NumeroColegiacion = command.NumeroColegiacion,
                FechaColegiacion = command.FechaColegiacion == default ? DateTime.Now : command.FechaColegiacion,
                CertificadoHabilitacionRuta = command.CertificadoHabilitacionRuta ?? string.Empty,
                Habilitado = command.Habilitado,
                MotivoNoHabilitado = command.MotivoNoHabilitado ?? string.Empty
            };

            await connection.ExecuteAsync("sp_PostulanteColegiatura_Mantenimiento", parametros, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<bool> MantenimientoIdiomaAsync(MantenimientoIdiomaCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var parametros = new
            {
                Accion = command.Accion,
                IdPostulanteIdioma = command.IdPostulanteIdioma,
                IdPostulante = command.IdPostulante,
                IdIdiomaCat = command.IdIdiomaCat,
                IdNivelHablaCat = command.IdNivelHablaCat,
                IdNivelLecturaCat = command.IdNivelLecturaCat,
                IdNivelEscrituraCat = command.IdNivelEscrituraCat
            };

            await connection.ExecuteAsync("sp_PostulanteIdioma_Mantenimiento", parametros, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<bool> MantenimientoOfimaticaAsync(MantenimientoOfimaticaCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var parametros = new
            {
                Accion = command.Accion,
                IdPostulanteOfimatica = command.IdPostulanteOfimatica,
                IdPostulante = command.IdPostulante,
                IdHerramientaCat = command.IdHerramientaCat,
                IdNivelCat = command.IdNivelCat
            };

            await connection.ExecuteAsync("sp_PostulanteOfimatica_Mantenimiento", parametros, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<bool> MantenimientoRequisitoEspecialAsync(MantenimientoRequisitoEspecialCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var parametros = new
            {
                Accion = command.Accion,
                IdRequisitoEspecial = command.IdRequisitoEspecial,
                IdPostulante = command.IdPostulante,
                IdTipoRequisitoCat = command.IdTipoRequisitoCat,
                IdCatalogoNivelDecanCat = command.IdCatalogoNivelDecanCat,
                DescripcionDocumento = command.DescripcionDocumento,
                NumeroRegistro = command.NumeroRegistro,
                FechaEmision = command.FechaEmision,
                FechaVencimiento = command.FechaVencimiento
            };

            await connection.ExecuteAsync("sp_PostulanteRequisitoEspecial_Mantenimiento", parametros, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<bool> GuardarInfoAdicionalAsync(GuardarInfoAdicionalCommand command)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var dtDepartamentos = new DataTable();
            dtDepartamentos.Columns.Add("id_departamento", typeof(string));

            if (command.DepartamentosIds != null)
            {
                foreach (var id in command.DepartamentosIds)
                {
                    dtDepartamentos.Rows.Add(id);
                }
            }

            var parametros = new DynamicParameters();
            parametros.Add("@IdPostulante", command.IdPostulante);
            parametros.Add("@DisponibilidadInterior", command.DisponibilidadInterior);
            parametros.Add("@DepartamentosSelected", dtDepartamentos.AsTableValuedParameter("dbo.UDTT_DepartamentosIds"));

            await connection.ExecuteAsync(
                "sp_Postulante_GuardarInfoAdicional",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }

        public async Task<bool> GuardarFirmaAsync(int idPostulante, byte[] archivoBytes, string tipoMime)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var parametros = new { IdPostulante = idPostulante, FirmaDigitalizada = archivoBytes, FirmaTipoMime = tipoMime };
            await connection.ExecuteAsync("sp_Postulante_ActualizarFirma", parametros, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<DatosPostulanteTokenDto?> RegistrarTokenRecuperacionAsync(string numDocumento, string token)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<DatosPostulanteTokenDto>(
                "sp_Postulante_RegistrarTokenRecuperacion",
                new { NumDocumento = numDocumento, TokenRecuperacion = token, MinutosExpiracion = 20 },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> RestablecerPasswordAsync(string token, string nuevoPasswordHash)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            int resultado = await connection.ExecuteScalarAsync<int>(
                "sp_Postulante_RestablecerPassword",
                new { TokenRecuperacion = token, NuevoPasswordHash = nuevoPasswordHash },
                commandType: CommandType.StoredProcedure
            );

            return resultado == 1;
        }

        public async Task<bool> GuardarDeclaracionesAsync(int idPostulante, List<GuardarDeclaracionItemDto> declaraciones)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var dtDeclaraciones = new DataTable();
            dtDeclaraciones.Columns.Add("id_declaracion_cat", typeof(int));
            dtDeclaraciones.Columns.Add("aceptado", typeof(bool));

            foreach (var d in declaraciones)
            {
                dtDeclaraciones.Rows.Add(d.IdDeclaracionCat, d.Aceptado);
            }

            var parametros = new DynamicParameters();
            parametros.Add("@IdPostulante", idPostulante);
            // Vinculamos el UDTT de SQL Server de manera limpia
            parametros.Add("@DeclaracionesSelected", dtDeclaraciones.AsTableValuedParameter("dbo.UDTT_PostulanteDeclaraciones"));

            await connection.ExecuteAsync(
                "sp_PostulanteDeclaracion_Guardar",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }

        public async Task<int> ObtenerTotalPostulacionesAnualAsync(int anio)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM sc_postulacion WHERE YEAR(fecha_postulacion) = @Anio";
            return await connection.ExecuteScalarAsync<int>(sql, new { Anio = anio });
        }

        public async Task<bool> InsertarPostulacionLocalAsync(int idPostulante, int idPlaza, int idEstadoCat, string codigoPostulacion)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            int idGenerado = await connection.ExecuteScalarAsync<int>(
                "sp_Postulacion_Registrar",
                new
                {
                    IdPostulante = idPostulante,
                    IdPlaza = idPlaza,
                    IdEstadoPostulacionCat = idEstadoCat,
                    CodigoPostulacion = codigoPostulacion
                },
                commandType: CommandType.StoredProcedure
            );

            return idGenerado > 0;
        }

        public async Task<bool> CrearNotificacionAsync(int idPostulante, string titulo, string mensaje, int idTipoAlertaCat)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            int filasAfectadas = await connection.ExecuteAsync(
                "sp_Notificacion_Insertar",
                new
                {
                    IdPostulante = idPostulante,
                    Titulo = titulo,
                    Mensaje = mensaje,
                    IdTipoAlertaCat = idTipoAlertaCat
                },
                commandType: CommandType.StoredProcedure
            );

            return filasAfectadas > 0;
        }

        public async Task<bool> InsertarTicketAsync(int idPostulante, int? idPlaza, int idTipoTicketCat, string asunto, string descripcion)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            int filasAfectadas = await connection.QuerySingleAsync<int>(
                "sp_SoporteTicket_Insertar",
                new
                {
                    IdPostulante = idPostulante,
                    IdPlaza = idPlaza,
                    IdTipoTicketCat = idTipoTicketCat,
                    Asunto = asunto,
                    Descripcion = descripcion
                },
                commandType: CommandType.StoredProcedure
            );

            return filasAfectadas > 0;
        }

        public async Task<bool> ActualizarRutaSustentoAsync(int idRegistro, string urlSustentoPdf, string seccion)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            var filasAfectadas = await connection.ExecuteScalarAsync<int>(
                "sp_Postulante_ActualizarSustento",
                new
                {
                    IdRegistro = idRegistro,
                    UrlSustentoPdf = urlSustentoPdf,
                    Seccion = seccion.ToUpper()
                },
                commandType: CommandType.StoredProcedure
            );

            return filasAfectadas > 0;
        }

        public async Task<string> EliminarRutaSustentoAsync(int idRegistro, string seccion)
        {
            using IDbConnection connection = _dbConnectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@IdRegistro", idRegistro);
            parameters.Add("@Seccion", seccion.ToUpper());
            parameters.Add("@RutaAntigua", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync("sp_Postulante_EliminarSustento", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<string>("@RutaAntigua");
        }
    }
}
