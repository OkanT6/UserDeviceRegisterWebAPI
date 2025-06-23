
using EruMobil.Application.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Devices.Exceptions.AccessTokenExceptions
{
    public class StaffTokenIsNotValidException:BaseException
    {
        public StaffTokenIsNotValidException() : base("Staff token is not valid.")
        {
        }
        public StaffTokenIsNotValidException(string message) : base(message)
        {
        }
        public override string ToString()
        {
            return $"StudentTokenIsNotValidException: {Message}";
        }
    }
}
