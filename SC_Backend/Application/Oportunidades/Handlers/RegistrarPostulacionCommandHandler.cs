using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Oportunidades.Commands;
using Domain.Entities;
using MediatR;
using Domain.Enums;

namespace Application.Oportunidades.Handlers
{
    public class RegistrarPostulacionCommandHandler : IRequestHandler<RegistrarPostulacionCommand, bool>
    {
        private readonly IPostulanteCommandService _commandService;

        public RegistrarPostulacionCommandHandler(IPostulanteCommandService commandService)
        {
            _commandService = commandService;
        }

        public async Task<bool> Handle(RegistrarPostulacionCommand request, CancellationToken cancellationToken)
        {
            // 1. 🚀 INSTANCIAMOS EL DOMINIO: Cargamos el objeto rico de negocio en memoria
            var domainPostulacion = new Postulacion(
                request.IdPostulante,
                request.IdPlaza,
                request.FechaFinPlaza,
                request.YaPostulo
            );

            // 2. 🚀 VALIDACIÓN DE DOMINIO: Evaluamos las invariantes antes de tocar la BD
            var (esValido, mensajeError) = domainPostulacion.ValidarReglasDePostulacion();
            if (!esValido)
            {
                throw new InvalidOperationException(mensajeError);
            }

            // 3. 🚀 GENERACIÓN DE DATOS DE DOMINIO: Creamos el correlativo oficial
            int totalPostulacionesAnio = await _commandService.ObtenerTotalPostulacionesAnualAsync(DateTime.Now.Year);
            string codigoGenerado = domainPostulacion.GenerarCodigoCorrelativo(totalPostulacionesAnio);

            int idEstadoInscritoCat = (int)EstadoPostulacion.Inscrito;

            // 4. 🚀 PERSISTENCIA EN INFRAESTRUCTURA: Guardamos con Dapper
            return await _commandService.InsertarPostulacionLocalAsync(
                request.IdPostulante,
                request.IdPlaza,
                idEstadoInscritoCat,
                codigoGenerado
            );
        }
    }
}
