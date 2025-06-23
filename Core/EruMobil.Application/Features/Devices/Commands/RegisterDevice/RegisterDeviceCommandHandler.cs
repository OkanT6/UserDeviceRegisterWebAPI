using EruMobil.Application.Bases;
using EruMobil.Application.Features.Devices.Rules;
using EruMobil.Application.Interfaces.AutoMapper;
using EruMobil.Application.Interfaces.ObisisService;
using EruMobil.Application.Interfaces.UnitOfWorks;
using EruMobil.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
        private readonly string apiKey;

        public RegisterDeviceCommandHandler(IConfiguration configuration,
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
            this.apiKey = configuration["ApiKey"]; // API key from configuration, if needed for other purposes
        }

        public async Task<RegisterDeviceCommandResponse> Handle( RegisterDeviceCommandRequest request, CancellationToken cancellationToken)
        {
            bool isNewUser = false;
            bool isNewDevice = false;
            string apiKeyValue = string.Empty;

            // 1. Kullanıcı doğrulama accessToken ile
            if (request.UserType?.ToLower() == "student")
            {
                bool isTokenValid =await obisisAPI.IsStudentTokenValid(request.AccesToken);

                if(isTokenValid == false)
                {

                    //throw new Exception("Invalid token");
                }
            }
            else if (request.UserType?.ToLower() == "staff")
            {
                

                bool isTokenValid = await peyosisAPI.IsStaffTokenValid(request.AccesToken);

                if (isTokenValid == false)
                {

                    //throw new Exception("Invalid token");
                }
            }
            else
            {
                throw new Exception("User type is not valid.");
            }

            // 2. Kullanıcı doğrulama apiKey ile
            if (!string.IsNullOrEmpty(request.CurrentApiKey))
            {
                if(request.CurrentApiKey != apiKey)
                {
                    throw new Exception("Invalid API key.");
                }



                var apiKeyEntity = await unitOfWork.GetReadRepository<ApiKey>()
                    .GetAsync(a => a.Key == request.CurrentApiKey, enableTracking: true);

                if (apiKeyEntity == null)
                    throw new Exception("API key not found.");

                var device = await unitOfWork.GetReadRepository<Device>()
                    .GetAsync(d => d.Id == apiKeyEntity.DeviceId, enableTracking: true);

                if (device == null)
                    throw new Exception("Device for the API key not found.");

                var user = await unitOfWork.GetReadRepository<User>()
                    .GetAsync(u => u.Id == device.UserId, enableTracking: true);

                if (user == null)
                    throw new Exception("User for the device not found.");

                // 🔁 Eğer API key revoke edilmişse ama aynı cihaz ve kullanıcı ise yeni bir API key üret
                if (apiKeyEntity.IsRevoked &&
                    device.UniqueDeviceIdentifier == request.UniqueDeviceIdentifier &&
                    user.BusinessIdentifier == request.BusinessIdentifier)
                {
                    var newApiKey = new ApiKey
                    {
                        Key = GenerateApiKey(),
                        DeviceId = device.Id,
                        IsRevoked = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    await unitOfWork.GetWriteRepository<ApiKey>().AddAsync(newApiKey);
                    await unitOfWork.SaveAsync();

                    return new RegisterDeviceCommandResponse
                    {
                        IsNewUser = false,
                        IsNewDevice = false,
                        ApiKey = newApiKey.Key
                    };
                }

                // ❌ Revoke edilmiş ama bilgiler uyuşmuyorsa → Güvenlik riski
                if (apiKeyEntity.IsRevoked)
                    throw new Exception("Revoked API key used from a mismatched device or user.");

                // ❌ Aktif API key ama cihaz ID uyuşmuyor → Potansiyel güvenlik riski
                if (device.UniqueDeviceIdentifier != request.UniqueDeviceIdentifier)
                    throw new Exception("API key does not match the requesting device. Possible security breach.");

                // Bildirim durumunu güncelle
                device.NotificationsIsActive = request.NotificationBelIsActive;

                //fcm token değeri değiştiyse güncelle
                if (device.FcmToken != request.FcmToken)
                {
                    device.FcmToken = request.FcmToken;
                }
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
                // Eğer ApiKey yoksa isteği reddeceğiz.
                //throw new Exception("API key is required for device registration.");  


                //


                // 3. Yeni kullanıcı ve/veya cihaz kaydı

                // Kullanıcıyı BusinessIdentifier ile bul
                var existingUser = await unitOfWork.GetReadRepository<User>()
                    .GetAsync(x => x.BusinessIdentifier == request.BusinessIdentifier, enableTracking: true);

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

                // Cihazı kontrol et
                var existingDevice = await unitOfWork.GetReadRepository<Device>()
                    .GetAsync(x =>
                        x.UniqueDeviceIdentifier == request.UniqueDeviceIdentifier &&
                        x.UserId == existingUser.Id, enableTracking: true);

                if (existingDevice != null)
                {
                    existingDevice.NotificationsIsActive = request.NotificationBelIsActive;
                    await unitOfWork.SaveAsync();

                    var existingApiKey = await unitOfWork.GetReadRepository<ApiKey>()
                        .GetAsync(k => k.DeviceId == existingDevice.Id && !k.IsRevoked, enableTracking: false);

                    return new RegisterDeviceCommandResponse
                    {
                        IsNewUser = isNewUser,
                        IsNewDevice = false,
                        ApiKey = existingApiKey?.Key ?? string.Empty
                    };
                }
                else
                {
                    //yeni cihaz kaydı

                    var device = mapper.Map<Device, RegisterDeviceCommandRequest>(request);
                    device.UserId = existingUser.Id;

                    await unitOfWork.GetWriteRepository<Device>().AddAsync(device);
                    await unitOfWork.SaveAsync();

                    isNewDevice = true;

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
