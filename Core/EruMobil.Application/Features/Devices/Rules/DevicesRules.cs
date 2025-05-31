using EruMobil.Application.Bases;
using EruMobil.Application.Features.Auth.Exceptions;
using EruMobil.Application.Features.Devices.Exceptions;
using EruMobil.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Devices.Rules
{
    public class DevicesRules:BaseRules
    {
        public Task DeviceShouldNotBeExisted(Device device)
        {
            if (device is not null) throw new DeviceIsAlreadyExistsException(device.UniqueDeviceIdentifier);
            return Task.CompletedTask;
        }

        public Task TargetUserMustExists(User user)
        {
            if (user is null) throw new TargetUserMustExistsException();
            return Task.CompletedTask;
        }
    }
}
