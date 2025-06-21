using EruMobil.Application.Bases;
using EruMobil.Application.Features.Devices.Rules;
using EruMobil.Application.Interfaces.AutoMapper;
using EruMobil.Application.Interfaces.ObisisService;
using EruMobil.Application.Interfaces.UnitOfWorks;
using EruMobil.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Devices.Commands.RegisterDevice
{
    public class RegisterDeviceCommandHandler : BaseHandler, IRequestHandler<RegisterDeviceCommandRequest, Unit>
    {
        private readonly IObisisAPI obisisAPI;
        private readonly IPeyosisAPI peyosisAPI;
        private readonly DevicesRules devicesRules;

        public RegisterDeviceCommandHandler(
            IObisisAPI obisisAPI,
            IPeyosisAPI peyosisAPI,
            DevicesRules devicesRules,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.obisisAPI = obisisAPI;
            this.peyosisAPI = peyosisAPI;
            this.devicesRules = devicesRules;
        }

        public async Task<Unit> Handle(RegisterDeviceCommandRequest request, CancellationToken cancellationToken)
        {
            // 1. Kullanıcı doğrulama
            if (request.UserType?.ToLower() == "student")
            {
                obisisAPI.IsStudentTokenValid(request.AccesToken);
            }
            else if (request.UserType?.ToLower() == "staff")
            {
                peyosisAPI.IsStaffTokenValid(request.AccesToken);
            }
            else
            {
                throw new Exception("User type is not valid.");
            }

            // 2. Kullanıcıyı BusinessIdentifier üzerinden ara
            var userRepo = unitOfWork.GetReadRepository<User>();
            var existingUser = await userRepo.GetAsync(x => x.BusinessIdentifier == request.BusinessIdentifier);

            // 3. Kullanıcı yoksa oluştur
            if (existingUser == null)
            {
                existingUser = new User
                {
                    BusinessIdentifier = request.BusinessIdentifier,
                    UserType = request.UserType,
                    access_token_when_register = request.AccesToken
                };

                await unitOfWork.GetWriteRepository<User>().AddAsync(existingUser);
                await unitOfWork.SaveAsync(); // ID üretimi için kaydet
            }

            // 4. Cihazı kontrol et
            var deviceRepo = unitOfWork.GetReadRepository<Device>();
            var existingDevice = await deviceRepo.GetAsync(x =>
                x.UniqueDeviceIdentifier == request.UniqueDeviceIdentifier &&
                x.UserId == existingUser.Id, enableTracking: true);

            if (existingDevice != null)
            {
                // 5. Cihaz zaten varsa sadece bildirimi güncelle
                existingDevice.NotificationsIsActive = request.NotificationBelIsActive;
                await unitOfWork.SaveAsync();
            }
            else
            {
                // 6. Yeni cihaz ekle
                var device = mapper.Map<Device, RegisterDeviceCommandRequest>(request);
                device.UserId = existingUser.Id;
                device.NotificationsIsActive = request.NotificationBelIsActive;

                await unitOfWork.GetWriteRepository<Device>().AddAsync(device);
                await unitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
