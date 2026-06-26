using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Admin.Commands;
using Application.Admin.Dtos;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Admin.Handlers
{
    public class AtenderTicketCommandHandler : IRequestHandler<AtenderTicketCommand, bool>
    {
        private readonly IAdminSoporteService _adminService;
        private readonly IPostulanteCommandService _notifService;

        public AtenderTicketCommandHandler(IAdminSoporteService adminService, IPostulanteCommandService notifService)
        {
            _adminService = adminService;
            _notifService = notifService;
        }

        public async Task<bool> Handle(AtenderTicketCommand request, CancellationToken cancellationToken)
        {
            TicketResolucionResultadoDto? infoTicket = null;

            // 🚀 LÓGICA DE TRANSICIÓN DE ESTADOS
            if (request.IdEstado == 1103) // RECEPCIONAR
            {
                infoTicket = await _adminService.RecepcionarTicketAsync(request.IdTicket);
            }
            else // EN EVALUACIÓN, ATENDIDO O IMPROCEDENTE
            {
                infoTicket = await _adminService.CambiarEstadoTicketAsync(request.IdTicket, request.RespuestaSoporte, request.IdEstado);
            }

            bool exito = infoTicket != null;

            if (exito)
            {
                try
                {
                    // Personalizamos la alerta que va al panel del Postulante según el hito
                    string tituloAlerta = $"Ticket #{request.IdTicket:D3} Actualizado";
                    string mensajeAlerta = request.IdEstado switch
                    {
                        1103 => $"Su solicitud con Asunto '{infoTicket!.Asunto}' ha sido RECIBIDA por el área técnica.",
                        1104 => $"Su solicitud con Asunto '{infoTicket!.Asunto}' ha pasado a EN EVALUACIÓN (Soporte Técnico está validando su caso).",
                        1105 => $"Su solicitud ha sido ATENDIDA. Respuesta oficial: {request.RespuestaSoporte}",
                        1106 => $"Su solicitud fue declarada IMPROCEDENTE. Sustento: {request.RespuestaSoporte}",
                        _ => $"Su ticket cambió de estado."
                    };

                    // Notificación al Postulante (Tipo 1 = Informativo)
                    await _notifService.CrearNotificacionAsync(infoTicket!.IdPostulante, tituloAlerta, mensajeAlerta, 1);
                }
                catch { /* Fail-safe */ }
            }

            return exito;
        }
    }
}
