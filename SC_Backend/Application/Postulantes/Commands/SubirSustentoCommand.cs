using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Postulantes.Commands
{
    public record SubirSustentoCommand(int IdRegistro, IFormFile Archivo, string Seccion) : IRequest<bool>;
}
