using EruMobil.Application.Bases;
using EruMobil.Application.Features.Devices.Rules;
using EruMobil.Application.Interfaces.AutoMapper;
using EruMobil.Application.Interfaces.ObisisService;
using EruMobil.Application.Interfaces.UnitOfWorks;
using EruMobil.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Devices.Commands.RegisterDevice
{
    public class RegisterDeviceCommandHandler : BaseHandler, IRequestHandler<RegisterDeviceCommandRequest, Unit>
    {
        private readonly IObisisAPI obisisAPI;
        private readonly DevicesRules devicesRules;

        public RegisterDeviceCommandHandler(IObisisAPI obisisAPI,IPeyosisAPI peyosisAPI,DevicesRules devicesRules,IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.obisisAPI = obisisAPI;
            this.devicesRules = devicesRules;
        }

        public async Task<Unit> Handle(RegisterDeviceCommandRequest request, CancellationToken cancellationToken)
        {
            //if (request.UserType.ToLower() == "student")
            //{
            //    obisisAPI.IsStudentTokenValid(request.AccesToken, request.UserId.ToString());
            //}

            await devicesRules.DeviceShouldNotBeExisted(await unitOfWork.GetReadRepository<Device>().GetAsync(x => x.UniqueDeviceIdentifier == request.UniqueDeviceIdentifier));

            await devicesRules.TargetUserMustExists(await unitOfWork.GetReadRepository<User>().GetAsync(x =>x.Id == request.UserId));

            Device device = mapper.Map<Device,RegisterDeviceCommandRequest>(request);

            await unitOfWork.GetWriteRepository<Device>().AddAsync(device);

            await unitOfWork.SaveAsync();

            return Unit.Value;

        }
    }
}
