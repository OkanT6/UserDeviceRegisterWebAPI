using EruMobil.Application.Bases;
using EruMobil.Application.Features.Auth.Rules;
using EruMobil.Application.Interfaces.AutoMapper;
using EruMobil.Application.Interfaces.MersisAPIs;
using EruMobil.Application.Interfaces.Repositories;
using EruMobil.Application.Interfaces.Tokens;
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
        private readonly IMersisAPI mersisAPI;
        private readonly IConfiguration configuration;
        private readonly ITokenService tokenService;
        private readonly AuthRules authRules;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        public RegisterCommandHandler(IMersisAPI mersisAPI,IConfiguration configuration, ITokenService tokenService,AuthRules authRules, UserManager<User> userManager, RoleManager<Role> roleManager,IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.mersisAPI = mersisAPI;
            this.configuration = configuration;
            this.tokenService = tokenService;
            this.authRules = authRules;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<RegisterCommandResponse> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            await mersisAPI.IsTcIsValidAsync(request.TCNo);

            string registerTypeTarget = request.UserType.ToLower();

            var existingUser = await userManager.Users
                .FirstOrDefaultAsync(u => u.TCNo == request.TCNo && u.UserType== registerTypeTarget);

            await authRules.UserShouldNotBeExisted(existingUser, registerTypeTarget);

            User user = mapper.Map<User, RegisterCommandRequest>(request);
            user.SecurityStamp = Guid.NewGuid().ToString(); //butona ilk basan kazanır

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
            user.Email = user.BusinessIdentifier + "@erciyes.edu.tr";
            user.UserName = user.Email;



            IdentityResult result = await userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = "user",
                        NormalizedName = "USER",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }

                if (!await roleManager.RoleExistsAsync("Student"))
                {
                    await roleManager.CreateAsync(new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = "student",
                        NormalizedName = "STUDENT",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }

                if (!await roleManager.RoleExistsAsync("Staff"))
                {
                    await roleManager.CreateAsync(new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = "staff",
                        NormalizedName = "STAFF",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }

                await userManager.AddToRoleAsync(user, "user");

                if (request.UserType.ToLower() == "student")
                {
                    await userManager.AddToRoleAsync(user, "student");
                }
                else if (request.UserType.ToLower() == "staff")
                {
                    await userManager.AddToRoleAsync(user, "staff");
                }

                // Token üret ve user objesine ata
                IList<string> roles = await userManager.GetRolesAsync(user);
                string userAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
                JwtSecurityToken token = await tokenService.CreateToken(user, roles, userAgent);
                string _token = new JwtSecurityTokenHandler().WriteToken(token);

                string refreshToken = tokenService.CreateRefreshToken();
                _ = int.TryParse(configuration["Jwt:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                // Propertileri ata
                user.access_token_when_register = _token;
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

                // Güncelle
                await userManager.UpdateAsync(user);
                await userManager.UpdateSecurityStampAsync(user);

                await userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", _token);

                return new()
                {
                    access_token_when_register = _token,
                    RefreshToken = refreshToken,
                    TokenExpiration = token.ValidTo
                };
            }

            throw new Exception("Kullanıcı oluşturulamadı.");
        }
    }
}
