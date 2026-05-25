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
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(IHttpClientFactory httpClientFactory, IDbConnectionFactory dbConnectionFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _dbConnectionFactory = dbConnectionFactory;
            _configuration = configuration;
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
            // Aquí ejecutas un SP como 'sp_ValidarLoginPostulante' con Dapper
            // Si es correcto, mapeas los menús fijos del Postulante que registramos en el SASI
            // Retornas el DTO con los submenús: 'postulante/dashboard', 'postulante/cv-datos', etc.
            return new LoginResultDto { Success = false }; // Implementación base
        }
    }
}
