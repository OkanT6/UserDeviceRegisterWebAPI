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
    public class RegisterDeviceCommandHandler : BaseHandler, IRequestHandler<RegisterDeviceCommandRequest, RegisterDeviceCommandResponse>
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

        public async Task<RegisterDeviceCommandResponse> Handle(RegisterDeviceCommandRequest request, CancellationToken cancellationToken)
        {
            bool isNewUser = false;
            bool isNewDevice = false;
            string apiKeyValue = string.Empty;

            if (!string.IsNullOrEmpty(request.CurrentApiKey))
            {
                // ApiKey ile cihazı doğrula
                var apiKeyEntity = await unitOfWork.GetReadRepository<ApiKey>()
                    .GetAsync(a => a.Key == request.CurrentApiKey && !a.IsRevoked, enableTracking: true);

                if (apiKeyEntity == null)
                    throw new Exception("Invalid or revoked API key.");

                // ApiKey'den cihaz bilgisine eriş
                var device = await unitOfWork.GetReadRepository<Device>()
                    .GetAsync(d => d.Id == apiKeyEntity.DeviceId, enableTracking: true);

                if (device == null)
                    throw new Exception("Device for the API key not found.");

                // **Cihaz kimliğini kontrol et**
                if (device.UniqueDeviceIdentifier != request.UniqueDeviceIdentifier)
                {
                    // Burada ya Exception fırlat, ya da log/alert oluştur
                    throw new Exception("API key does not match the requesting device. Possible security breach.");
                    // Alternatif: Log'a yaz, alert gönder vs.
                }

                // Kullanıcıyı da alalım
                var user = await unitOfWork.GetReadRepository<User>()
                    .GetAsync(u => u.Id == device.UserId, enableTracking: true);

                if (user == null)
                    throw new Exception("User for the device not found.");

                // Token doğrulaması isteğe bağlı ama tavsiye edilir
                if (user.UserType?.ToLower() == "student")
                {
                    await obisisAPI.IsStudentTokenValid(request.AccesToken);
                }
                else if (user.UserType?.ToLower() == "staff")
                {
                    await peyosisAPI.IsStaffTokenValid(request.AccesToken);
                }
                else
                {
                    throw new Exception("User type is not valid.");
                }

                // Bildirim durumunu güncelle
                device.NotificationsIsActive = request.NotificationBelIsActive;
                await unitOfWork.SaveAsync();

                return new RegisterDeviceCommandResponse
                {
                    IsNewUser = false,
                    IsNewDevice = false,
                    ApiKey = request.CurrentApiKey
                };
            }

            else
            {
                // İlk kayıt işlemi

                // 1. Kullanıcı doğrulama
                if (request.UserType?.ToLower() == "student")
                {
                    await obisisAPI.IsStudentTokenValid(request.AccesToken);
                }
                else if (request.UserType?.ToLower() == "staff")
                {
                    await peyosisAPI.IsStaffTokenValid(request.AccesToken);
                }
                else
                {
                    throw new Exception("User type is not valid.");
                }

                // 2. Kullanıcıyı BusinessIdentifier üzerinden ara
                var userRepo = unitOfWork.GetReadRepository<User>();
                var existingUser = await userRepo.GetAsync(x => x.BusinessIdentifier == request.BusinessIdentifier, enableTracking: true);

                // 3. Kullanıcı yoksa oluştur
                if (existingUser == null)
                {
                    existingUser = new User
                    {
                        BusinessIdentifier = request.BusinessIdentifier,
                        UserType = request.UserType,
                        access_token_when_register = request.AccesToken,
                    };

                    isNewUser = true;

                    await unitOfWork.GetWriteRepository<User>().AddAsync(existingUser);
                    await unitOfWork.SaveAsync();
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
                    var device = mapper.Map<Device,RegisterDeviceCommandRequest>(request);
                    device.UserId = existingUser.Id;

                    try
                    {
                        await unitOfWork.GetWriteRepository<Device>().AddAsync(device);
                        await unitOfWork.SaveAsync();
                    }
                    catch (Exception ex)
                    {
                        var inner = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                        throw new Exception($"Device saving failed: {ex.Message} - Inner: {inner}");
                    }

                    isNewDevice = true;

                    // ✔️ ApiKey oluştur ve kaydet
                    var apiKey = new ApiKey
                    {
                        Key = GenerateApiKey(),
                        DeviceId = device.Id,
                        IsRevoked = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    await unitOfWork.GetWriteRepository<ApiKey>().AddAsync(apiKey);
                    await unitOfWork.SaveAsync();

                    apiKeyValue = apiKey.Key;
                }

                return new RegisterDeviceCommandResponse
                {
                    IsNewUser = isNewUser,
                    IsNewDevice = isNewDevice,
                    ApiKey = apiKeyValue
                };
            }
        }


        private string GenerateApiKey()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}
