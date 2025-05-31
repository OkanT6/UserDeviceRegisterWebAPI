using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Commands.Revoke
{
    public class RevokeCommandRequest:IRequest<Unit>
    {
        [DefaultValue("01971841-f337-752f-89c1-8bf245a2521c")]

        public Guid UserId { get; set; } // Kullanıcının ID'si
    }
}
