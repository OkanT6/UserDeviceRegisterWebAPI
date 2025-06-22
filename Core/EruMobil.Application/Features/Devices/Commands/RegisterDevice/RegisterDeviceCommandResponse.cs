using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Devices.Commands.RegisterDevice
{
    public class RegisterDeviceCommandResponse
    {
        public bool IsNewUser { get; set; }
        public bool IsNewDevice { get; set; }
        public string ApiKey { get; set; } = string.Empty;
    }
}
