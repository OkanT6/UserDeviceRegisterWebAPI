using EruMobil.Application.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Devices.Exceptions
{
    public class TargetUserMustExistsException:BaseException
    {
        public TargetUserMustExistsException() : base("Target user must exist.")
        {
        }
    }
}
