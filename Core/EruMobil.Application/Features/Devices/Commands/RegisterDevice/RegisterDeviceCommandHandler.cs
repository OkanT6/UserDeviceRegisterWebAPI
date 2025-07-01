using EruMobil.Application.Bases;
using EruMobil.Application.Features.Devices.Rules;
using EruMobil.Application.Interfaces.AutoMapper;
using EruMobil.Application.Interfaces.ObisisService;
using EruMobil.Application.Interfaces.UnitOfWorks;
using EruMobil.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Devices.Commands.RegisterDevice
{
    public class RegisterDeviceCommandHandler : BaseHandler, IRequestHandler<RegisterDeviceCommandRequest, RegisterDeviceCommandResponse>
    {
        private readonly ILogger<RegisterDeviceCommandHandler> logger;
        private readonly IObisisAPI obisisAPI;
        private readonly IPeyosisAPI peyosisAPI;
        private readonly DevicesRules devicesRules;
        private readonly string apiKey;

        public RegisterDeviceCommandHandler(ILogger<RegisterDeviceCommandHandler> logger, IConfiguration configuration,
            IObisisAPI obisisAPI,
            IPeyosisAPI peyosisAPI,
            DevicesRules devicesRules,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.logger = logger;
            this.obisisAPI = obisisAPI;
            this.peyosisAPI = peyosisAPI;
            this.devicesRules = devicesRules;
            this.apiKey = configuration["ApiKey"]; // API key from configuration, if needed for other purposes
        }

        public async Task<RegisterDeviceCommandResponse> Handle(RegisterDeviceCommandRequest request, CancellationToken cancellationToken)
        {
            bool isNewUser = false;
            bool isNewDevice = false;
            string apiKeyValue = string.Empty;

            // 1. Kullanıcı doğrulama accessToken ile
            if (request.UserType?.ToLower() == "student")
            {
                // Obisis API'si üzerinden öğrenci doğrulaması yapıyoruz
                // Development ortamında gerçek ObisisAPI url'si kullanılmadığı için her halükülarda 404 hatası dönecektir
                // Bu yüzden alttaki if bloğunda'da herhangi bir işlem yapılmamaktadır ama production ortamında yorum satırı kaldırılıp yapılmalıdır.
                bool isTokenValid = await obisisAPI.IsStudentTokenValid(request.AccesToken);

                if (isTokenValid == false)
                {

                    // Production ortamında bu kısımda bir exception fırlatabiliriz ve loglamaya dahil edebiliriz
                    //throw new Exception("Invalid token");  
                }
            }
            else if (request.UserType?.ToLower() == "staff")
            {

                // Peyosis API'si üzerinden personel doğrulaması yapıyoruz
                // Development ortamında gerçek PeyosisAPI url'si kullanılmadığı için her halükülarda 404 hatası dönecektir
                // Bu yüzden alttaki if bloğunda'da herhangi bir işlem yapılmamaktadır ama production ortamında yorum satırı kaldırılıp yapılmalıdır.
                bool isTokenValid = await peyosisAPI.IsStaffTokenValid(request.AccesToken);

                if (isTokenValid == false)
                {
                    // Production ortamında bu kısımda bir exception fırlatabiliriz ve loglamaya dahil edebiliriz
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
                //Api Key yanlış ise tespit edeceğiz (Loglama yapıyoruz)
                if (request.CurrentApiKey != apiKey)
                {
                    //(Loglama yapıyoruz)
                    var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                    logger.LogWarning("Invalid API key attempt from {IP} for {BusinessIdentifier}", ip, request.BusinessIdentifier);
                    throw new Exception("Invalid API key.");

                }
                // Api Key doğrusu ise gerekli kayıt işlemleri başlatılır

                var usingDevice = await unitOfWork.GetReadRepository<Device>()
                    .GetAsync(d => d.UniqueDeviceIdentifier == request.UniqueDeviceIdentifier, enableTracking: true);
                var loggedInUser = await unitOfWork.GetReadRepository<User>()
                    .GetAsync(u => u.BusinessIdentifier == request.BusinessIdentifier, enableTracking: true);

                

                if (usingDevice == null)
                {
                    //device kaydı yapacağız loggedInUser'a

                    //Önce kullanıcıyı kontrol edeceğiz

                    
                    if (loggedInUser == null)
                    {
                        //LoggedInUser kayıtlı değilmiş demek

                        // Yeni kullanıcı oluştur
                        loggedInUser = new User
                        {
                            BusinessIdentifier = request.BusinessIdentifier,
                            UserType = request.UserType,
                            access_token_when_register = request.AccesToken,
                        };
                        isNewUser = true;
                        await unitOfWork.GetWriteRepository<User>().AddAsync(loggedInUser);
                        await unitOfWork.SaveAsync();


                        // Sonra o kullanıcını üstüne cihaz kaydı da yapacağız

                        var device = mapper.Map<Device, RegisterDeviceCommandRequest>(request);
                        device.UserId = loggedInUser.Id;

                        await unitOfWork.GetWriteRepository<Device>().AddAsync(device);
                        await unitOfWork.SaveAsync();

                        isNewDevice = true;
                    }
                    else
                    {
                        // Kullanıcı zaten var, sadece device kaydı yapacağız

                        //yeni cihaz kaydı

                        var device = mapper.Map<Device, RegisterDeviceCommandRequest>(request);
                        device.UserId = loggedInUser.Id;

                        await unitOfWork.GetWriteRepository<Device>().AddAsync(device);
                        await unitOfWork.SaveAsync();

                        isNewDevice = true;

                    }


                }
                else
                {
                    // Cihaz ve kullanıcı zaten kayıtlı, bildirim güncellemesi yapacağız
                    if (usingDevice.UserId == loggedInUser.Id)
                    {
                        usingDevice.NotificationsIsActive = request.NotificationsIsActive;
                        await unitOfWork.SaveAsync();
                    }
                    // Cihaz başka bir kullanıcıya ait, bu durumda alarm verip loglama yapacağız
                    else
                    {
                        var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

                        //(Loglama yapıyoruz)
                        logger.LogWarning(
                "Device conflict! Device {DeviceId} belongs to user {OwnerId} but accessed by {RequestedUser} from {IP} ip & from {BusinessIdentifier} businessIdentifer",
                usingDevice.Id, usingDevice.UserId, loggedInUser.Id, ip,loggedInUser.BusinessIdentifier);

                        throw new Exception("Device is already registered to another user.");


                    }

                }
            }
            else
            {            
                
                //(Loglama yapabiliriz)
                //Eğer ApiKey yoksa isteği reddeceğiz.

                var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                logger.LogWarning("No sending api key attempt from {IP} for {BusinessIdentifier}", ip, request.BusinessIdentifier);
                throw new Exception("API key is required for device registration.");

            }

            return new RegisterDeviceCommandResponse
            {
                IsNewUser = isNewUser,
                IsNewDevice = isNewDevice,
            };
        } 
    }
}
