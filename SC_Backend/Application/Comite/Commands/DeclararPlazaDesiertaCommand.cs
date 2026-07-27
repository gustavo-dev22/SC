using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comite.Commands
{
    public record DeclararPlazaDesiertaCommand(int IdPlaza, int IdMotivoDesiertaCat, string SustentoDesierta, string UsuarioDeclara) : IRequest<bool>;
}
