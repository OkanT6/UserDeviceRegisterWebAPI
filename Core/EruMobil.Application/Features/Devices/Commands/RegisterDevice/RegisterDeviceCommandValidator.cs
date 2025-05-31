using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Devices.Commands.RegisterDevice
{
    public class RegisterDeviceCommandValidator:AbstractValidator<RegisterDeviceCommandRequest>
    {
        public RegisterDeviceCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID cannot be empty.")
                .Must(x => x != Guid.Empty).WithMessage("User ID must be a valid GUID.");
            RuleFor(x => x.UniqueDeviceIdentifier)
                .NotEmpty().WithMessage("Unique Device Identifier cannot be empty.")
                .Length(1, 100).WithMessage("Unique Device Identifier must be between 1 and 100 characters long.");
            RuleFor(x => x.DeviceName)
                .NotEmpty().WithMessage("Device Name cannot be empty.")
                .Length(1, 100).WithMessage("Device Name must be between 1 and 100 characters long.");
            RuleFor(x => x.Platform)
                .NotEmpty().WithMessage("Platform cannot be empty.")
                .Must(p => p == "android" || p == "ios").WithMessage("Platform must be either 'android' or 'ios'.");
            RuleFor(x => x.FcmToken)
                .NotEmpty().WithMessage("FCM Token cannot be empty.")
                .Length(1, 200).WithMessage("FCM Token must be between 1 and 200 characters long.");
            RuleFor(x => x.AppVersion)
                .NotEmpty().WithMessage("App Version cannot be empty.")
                .Length(1, 50).WithMessage("App Version must be between 1 and 50 characters long.");

        }
    }


}
