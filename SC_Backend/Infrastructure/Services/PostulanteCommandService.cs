using Application.Common.Interfaces;
using Application.Postulantes.Commands;
using Dapper;
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
    }
}
