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
            var domainPostulacion = new Postulacion(
                request.IdPostulante,
                request.IdPlaza,
                request.FechaFinPlaza,
                request.YaPostulo
            );

            var (esValido, mensajeError) = domainPostulacion.ValidarReglasDePostulacion();
            if (!esValido)
            {
                throw new InvalidOperationException(mensajeError);
            }

            int totalPostulacionesAnio = await _commandService.ObtenerTotalPostulacionesAnualAsync(DateTime.Now.Year);
            string codigoGenerado = domainPostulacion.GenerarCodigoCorrelativo(totalPostulacionesAnio);

            int idEstadoInscritoCat = (int)EstadoPostulacion.Inscrito;

            bool resultadoPostulacion = await _commandService.InsertarPostulacionLocalAsync(
                request.IdPostulante,
                request.IdPlaza,
                idEstadoInscritoCat,
                codigoGenerado
            );

            if (resultadoPostulacion)
            {
                try
                {
                    string tituloNotif = "Postulación Recibida Conforme";
                    string mensajeNotif = $"¡Éxito! Su postulación fue registrada correctamente bajo el N° de Expediente: {codigoGenerado}. Puede descargar su constancia en la sección de historial.";

                    // Tipo de Alerta Catálogo: 3 (Éxito / Verde)
                    await _commandService.CrearNotificacionAsync(request.IdPostulante, tituloNotif, mensajeNotif, 3);
                }
                catch
                {
                    // Mecanismo preventivo (Fail-safe): Si falla la inserción de la alerta por red o timeout,
                    // no dañamos ni tiramos abajo la postulación principal que ya se guardó con éxito.
                }
            }

            return resultadoPostulacion;
        }
    }
}
