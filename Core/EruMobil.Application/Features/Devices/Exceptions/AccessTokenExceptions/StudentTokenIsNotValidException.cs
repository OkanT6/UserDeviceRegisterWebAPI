using EruMobil.Application.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Devices.Exceptions.AccessTokenExceptions
{
    public class StudentTokenIsNotValidException:BaseException
    {
        public StudentTokenIsNotValidException() : base("Student token is not valid.")
        {
        }
        public StudentTokenIsNotValidException(string message) : base(message)
        {
        }
        public override string ToString()
        {
            return $"StudentTokenIsNotValidException: {Message}";
        }
    }
}
