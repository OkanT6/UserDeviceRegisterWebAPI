using EruMobil.Application.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Devices.Exceptions
{
    public class DeviceIsAlreadyExistsException : BaseException
    {
        public DeviceIsAlreadyExistsException(string uniqueDeviceIdentifier)
            : base($"A device with the unique identifier '{uniqueDeviceIdentifier}' already exists.")
        {
        }
    }
}
