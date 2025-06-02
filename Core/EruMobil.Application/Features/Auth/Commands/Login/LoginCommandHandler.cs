using EruMobil.Application.Bases;
using EruMobil.Application.Interfaces.AutoMapper;
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
using EruMobil.Application.Features.Auth.Rules;

namespace EruMobil.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : BaseHandler, IRequestHandler<LoginCommandRequest, LoginCommandResponse>
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly ITokenService tokenService;
        private readonly AuthRules authRules;
        public LoginCommandHandler(UserManager<User> userManager, IConfiguration configuration, ITokenService tokenService, AuthRules authRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.tokenService = tokenService;
            this.authRules = authRules;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            
            User user = await userManager.Users.FirstOrDefaultAsync(u=>u.BusinessIdentifier==request.StudentNumber);
            bool chechPassword = await userManager.CheckPasswordAsync(user, request.Password);
            await authRules.StudentNumberAndPasswordShouldBeMatched(user, chechPassword);

            await authRules.LoginedDeviceMustBeMatchedOrNotRegistered(user,request);

            IList<string> roles = await userManager.GetRolesAsync(user);

            string userAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            JwtSecurityToken token = await tokenService.CreateToken(user, roles, userAgent);
            string refreshToken = tokenService.CreateRefreshToken();

            _ = int.TryParse(configuration["Jwt:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenEndDate = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

            await userManager.UpdateAsync(user);

            await userManager.UpdateSecurityStampAsync(user);

            string _token = new JwtSecurityTokenHandler().WriteToken(token);

            await userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", _token);

            return new()
            {
                Token = _token,
                RefreshToken = refreshToken,
                TokenExpiration = token.ValidTo
            };


        }
    }
}
