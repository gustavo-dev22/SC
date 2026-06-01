using System.Net.Http.Json;
using Application.Auth.Commands;
using Application.Auth.Dto;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using static Application.Common.Models.Integration.Sasi.SasiIntegrationModels;

namespace Application.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResultDto>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IPostulanteQueryService _postulanteQueryService;

        public LoginCommandHandler(IHttpClientFactory httpClientFactory, IConfiguration configuration, IPostulanteQueryService postulanteQueryService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _postulanteQueryService = postulanteQueryService;
        }

        public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // CASO A: USUARIO INTERNO (VALIDA CONTRA EL ENDPOINT DE SASI)
            if (!request.IsExternal)
            {
                var sasiUrl = _configuration["IntegrationServices:SasiAuthUrl"];
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsJsonAsync(sasiUrl, new
                {
                    userName = request.Username,
                    password = request.Password
                }, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return new LoginResultDto { Success = false };

                var sasiResult = await response.Content.ReadFromJsonAsync<SasiResponseModel>(cancellationToken: cancellationToken);

                if (sasiResult == null || !sasiResult.Success)
                    return new LoginResultDto { Success = false };

                // Filtrar específicamente por nuestro Sistema de Convocatorias (ID = 6)
                var sistemaConvocatorias = sasiResult.Usuario.Sistemas.FirstOrDefault(s => s.Id == 6);
                if (sistemaConvocatorias == null || !sistemaConvocatorias.Activo)
                    return new LoginResultDto { Success = false }; // No tiene acceso a este sistema específico

                // Tomamos el rol activo del sistema
                var rolActivo = sistemaConvocatorias.Roles.FirstOrDefault(r => r.Activo);

                return new LoginResultDto
                {
                    Success = true,
                    Token = sasiResult.Token,
                    NombreCompleto = sasiResult.Usuario.NombreCompleto,
                    Rol = rolActivo?.NombreRol ?? "Usuario Interno",
                    Menus = rolActivo?.Objetos.Select(o => new MenuObjetoDto
                    {
                        IdObjeto = o.IdObjeto,
                        IdPadre = o.IdPadre,
                        Nombre = o.Nombre,
                        Tipo = o.Tipo,
                        Url = o.Url,
                        Icono = o.Icono
                    }).ToList() ?? new List<MenuObjetoDto>()
                };
            }

            // CASO B: USUARIO EXTERNO (POSTULANTE - CONSULTA PROCEDIMIENTO ALMACENADO MEDIANTE DAPPER)
            // Aquí llamarías a tu SP local para validar credenciales públicas del ciudadano
            return await ValidarPostulanteLocal(request);
        }

        private async Task<LoginResultDto> ValidarPostulanteLocal(LoginCommand request)
        {
            // 1. Lectura mediante el SP en Infrastructure
            var postulante = await _postulanteQueryService.ObtenerByDocumentoAsync(request.Username);

            if (postulante == null)
            {
                return new LoginResultDto { Success = false, MensajeError = "El número de documento no se encuentra registrado." };
            }

            // 2. Verificación criptográfica de la clave
            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, (string)postulante.PasswordHash);
            if (!isValid)
            {
                return new LoginResultDto { Success = false, MensajeError = "La contraseña ingresada es incorrecta." };
            }

            // 3. Generación del Token local sin dependencias externas
            string tokenReal = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"POSTULANTE-{postulante.IdPostulante}-{DateTime.UtcNow.Ticks}"));

            // 4. Estructura Plana Oficial sacada de tu árbol de SASI
            var menusOficialesPostulante = new List<MenuObjetoDto>
            {
                new MenuObjetoDto { IdObjeto = 100, IdPadre = null, Nombre = "Mi Panel", Tipo = "M", Url = "", Icono = "dashboard" },
                new MenuObjetoDto { IdObjeto = 101, IdPadre = 100, Nombre = "Resumen de Postulaciones", Tipo = "S", Url = "/postulante/resumen", Icono = "assignment_late" },
                new MenuObjetoDto { IdObjeto = 102, IdPadre = 100, Nombre = "Alertas y Notificaciones", Tipo = "S", Url = "/postulante/notificaciones", Icono = "notifications_active" },

                new MenuObjetoDto { IdObjeto = 200, IdPadre = null, Nombre = "Mi Curriculum", Tipo = "M", Url = "", Icono = "contact_page" },
                new MenuObjetoDto { IdObjeto = 201, IdPadre = 200, Nombre = "Datos Personales", Tipo = "S", Url = "/postulante/datos-personales", Icono = "manage_accounts" },
                new MenuObjetoDto { IdObjeto = 202, IdPadre = 200, Nombre = "Formación Académica", Tipo = "S", Url = "/postulante/formacion", Icono = "school" },
                new MenuObjetoDto { IdObjeto = 203, IdPadre = 200, Nombre = "Experiencia Laboral", Tipo = "S", Url = "/postulante/experiencia", Icono = "business_center" },

                new MenuObjetoDto { IdObjeto = 300, IdPadre = null, Nombre = "Oportunidades", Tipo = "M", Url = "", Icono = "local_activity" },
                new MenuObjetoDto { IdObjeto = 301, IdPadre = 300, Nombre = "Buscar Plazas Vacantes", Tipo = "S", Url = "/postulante/buscar-plazas", Icono = "travel_explore" },

                new MenuObjetoDto { IdObjeto = 400, IdPadre = null, Nombre = "Soporte", Tipo = "M", Url = "", Icono = "help_center" },
                new MenuObjetoDto { IdObjeto = 401, IdPadre = 400, Nombre = "Consultas y Reclamos", Tipo = "S", Url = "/postulante/consultas-reclamos", Icono = "support_agent" }
            };

            return new LoginResultDto
            {
                Success = true,
                Token = tokenReal,
                NombreCompleto = $"{postulante.Nombres} {postulante.ApellidoPaterno} {postulante.ApellidoMaterno}",
                Rol = "POSTULANTE",
                Menus = menusOficialesPostulante
            };
        }
    }
}
