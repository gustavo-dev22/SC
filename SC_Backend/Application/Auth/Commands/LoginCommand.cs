using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Auth.Dto;
using MediatR;

namespace Application.Auth.Commands
{
    public record LoginCommand(string Username, string Password, bool IsExternal) : IRequest<LoginResultDto>;
}
