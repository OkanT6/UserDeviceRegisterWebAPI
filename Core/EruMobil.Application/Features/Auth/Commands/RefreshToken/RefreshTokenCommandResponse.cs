using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandResponse
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }
}
