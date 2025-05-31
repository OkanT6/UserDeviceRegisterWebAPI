using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Commands.Login
{
    public class LoginCommandRequest:IRequest<LoginCommandResponse>
    {
        [DefaultValue("1030511385")]
        public string StudentNumber { get; set; }
        [DefaultValue("Okan1234")]

        public string Password { get; set; }
    }
}
