using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandResponse
    {
        public string access_token_when_register { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}
