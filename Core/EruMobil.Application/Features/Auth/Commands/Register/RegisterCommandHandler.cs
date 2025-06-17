using EruMobil.Application.Bases;
using EruMobil.Application.Features.Auth.Rules;
using EruMobil.Application.Interfaces.AutoMapper;
using EruMobil.Application.Interfaces.Repositories;
using EruMobil.Application.Interfaces.UnitOfWorks;
using EruMobil.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : BaseHandler, IRequestHandler<RegisterCommandRequest, RegisterCommandResponse>
    {
        private readonly IConfiguration configuration;
        private readonly AuthRules authRules;
        public RegisterCommandHandler( IConfiguration configuration,AuthRules authRules,IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.configuration = configuration;
            this.authRules = authRules;
        }

        public async Task<RegisterCommandResponse> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            //await mersisAPI.IsTcIsValidAsync(request.TCNo);

            string registerTypeTarget = request.UserType.ToLower();

            var existingUser = await unitOfWork.GetReadRepository<User>()
                .GetAsync(u => u.BusinessIdentifier == request.BusinessIdentifier && u.UserType == registerTypeTarget);
            //    .FirstOrDefaultAsync(u => u.TCNo == request.TCNo && u.UserType== registerTypeTarget);

            await authRules.UserShouldNotBeExisted(existingUser, registerTypeTarget);

            User user = mapper.Map<User, RegisterCommandRequest>(request);
            //user.SecurityStamp = Guid.NewGuid().ToString(); //butona ilk basan kazanır

            if (user.UserType.ToLower() == "student")
            {
                user.BusinessIdentifier = new BusinessIdentitfierGenerator().CreateStudentNumber();
            }
            else if (user.UserType.ToLower() == "staff")
            {
                user.BusinessIdentifier = new BusinessIdentitfierGenerator().CreateStaffNumber();
            }
            else if (user.UserType.ToLower() == "user")
            {
                user.BusinessIdentifier = new BusinessIdentitfierGenerator().CreateDefaultUserNumber();
            }
            else
                throw new ArgumentException("Invalid User Type");
            //user.Email = user.BusinessIdentifier + "@erciyes.edu.tr";
            //user.UserName = user.Email;



            

            throw new Exception("Kullanıcı oluşturulamadı.");
        }
    }
}
