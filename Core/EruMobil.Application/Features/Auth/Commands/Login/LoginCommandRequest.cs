using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Commands.Login
{
    public class LoginCommandRequest:IRequest<LoginCommandResponse>
    {
        [DefaultValue("1030511385")]
        public string StudentNumber { get; set; }
        [DefaultValue("Okan1234")]

        public string Password { get; set; }


        [DefaultValue("your_DeviceName_value")]

        public string DeviceName { get; set; }
        [DefaultValue("your_UniqueDeviceIdentifier_value")]
        public string UniqueDeviceIdentifier { get; set; }
        [DefaultValue("your_Platform_value")]

        public string Platform { get; set; } // "android" veya "ios"
        [DefaultValue("your_FcmTokene_value")]

        public string FcmToken { get; set; }
        [DefaultValue("your_AppVersion_value")]

        public string AppVersion { get; set; }
    }
}
