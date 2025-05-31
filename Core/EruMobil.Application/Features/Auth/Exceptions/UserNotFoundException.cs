using EruMobil.Application.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Exceptions
{
    public class UserNotFoundException: BaseException
    {
        public UserNotFoundException() : base("User not found.")
        {
        }
    }
}
