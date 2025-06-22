using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Devices.Commands.RegisterDevice
{
    public class RegisterDeviceCommandRequest : IRequest<RegisterDeviceCommandResponse>
    {
        public string? CurrentApiKey { get; set; }
        public string DeviceName { get; set; }
        public string UniqueDeviceIdentifier { get; set; }
        public string Platform { get; set; } // "android" veya "ios"
        public string FcmToken { get; set; }
        public string AppVersion { get; set; }

        public string AccesToken { get; set; }

        public string UserType { get; set; } // "student" veya "staff"

        public string BusinessIdentifier { get; set; }


        public bool NotificationBelIsActive { get; set; }


    }

    /*
     public class DeviceInfoDto
{
     public string DeviceName { get; set; }
    public string UniqueDeviceIdentifier { get; set; }
    public string Platform { get; set; } // "android" veya "ios"
    public string FcmToken { get; set; }
    public string AppVersion { get; set; }
   
}
     */
}
