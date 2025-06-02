using EruMobil.Application.Bases;
using EruMobil.Application.Features.Auth.Commands.Login;
using EruMobil.Application.Features.Auth.Exceptions;
using EruMobil.Application.Features.Devices.Commands.RegisterDevice;
using EruMobil.Application.Interfaces.UnitOfWorks;
using EruMobil.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Rules
{
    public class AuthRules : BaseRules
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMediator mediator;

        public AuthRules(IUnitOfWork unitOfWork,IMediator mediator)
        {
            this.unitOfWork = unitOfWork;
            this.mediator = mediator;
        }
        public Task UserShouldNotBeExisted(User user, string registerTypeTarget)
        {
            if (user is not null && registerTypeTarget == user.UserType) throw new UserAlreadyExistsException();
            return Task.CompletedTask;
        }

        public Task StudentNumberAndPasswordShouldBeMatched(User user, bool checkPassowrd)
        {
            if (user is null || !checkPassowrd) throw new StudentNumberAndPasswordShouldBeMatched();
            return Task.CompletedTask;
        }

        public Task RefreshTokenShouldNotBeExpired(DateTime? expiryDate)
        {
            if (DateTime.Now >= expiryDate) throw new RefreshTokenShouldNotBeExpiredException();
            return Task.CompletedTask;
        }


        public Task UserShoulExists(User user)
        {
            if (user is null) throw new UserNotFoundException();
            return Task.CompletedTask;
        }

        public async Task LoginedDeviceMustBeMatchedOrNotRegistered(User user, LoginCommandRequest loginCommandRequest)
        {

            
            Device device = await unitOfWork.GetReadRepository<Device>().GetAsync(Device => Device.UniqueDeviceIdentifier == loginCommandRequest.UniqueDeviceIdentifier);

            if (device is null)
            {
                // Device bulunamadı, yeni device kaydet
                var registerDeviceCommandRequest = new RegisterDeviceCommandRequest
                {
                    UserId = user.Id,
                    UniqueDeviceIdentifier = loginCommandRequest.UniqueDeviceIdentifier,
                    DeviceName = loginCommandRequest.DeviceName, // veya parametre olarak alabilirsiniz
                    Platform = loginCommandRequest.Platform, // veya User-Agent'tan çıkarabilirsiniz
                    FcmToken = loginCommandRequest.FcmToken, // İlk kayıtta boş, sonra update edilebilir
                    AppVersion = loginCommandRequest.AppVersion // veya parametre olarak alabilirsiniz
                };

                await mediator.Send(registerDeviceCommandRequest);


            }
            else if (device.UserId==user.Id)
            {
                return;            }

            else         
                throw new LoginedDeviceMustBeMatchedOrNotRegisteredException();   

        }


    }
}
