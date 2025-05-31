using EruMobil.Application.Bases;
using EruMobil.Application.Features.Auth.Rules;
using EruMobil.Application.Interfaces.AutoMapper;
using EruMobil.Application.Interfaces.Tokens;
using EruMobil.Application.Interfaces.UnitOfWorks;
using EruMobil.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : BaseHandler, IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
    {
        private readonly UserManager<User> userManager;
        private readonly AuthRules authRules;
        private readonly ITokenService tokenService;
        public RefreshTokenCommandHandler(UserManager<User> userManager, AuthRules authRules,ITokenService tokenService, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.userManager = userManager;
            this.authRules = authRules;
            this.tokenService = tokenService;
        }

        public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
        {
            ClaimsPrincipal? principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            //string email = principal?.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
            string NameIdentifier = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            User? user = userManager.Users.FirstOrDefault(u=>u.BusinessIdentifier== NameIdentifier);

            IList<string> roles = await userManager.GetRolesAsync(user);

            //if(DateTime.Now >= user.RefreshTokenEndDate)
            //{
            //    throw new Exception("Refresh token expired!/n Log in again, please");
            //}

            await authRules.RefreshTokenShouldNotBeExpired(user.RefreshTokenEndDate);

            string userAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            JwtSecurityToken newAccessToken = await tokenService.CreateToken(user, roles, userAgent); //.Result.ToString();
            string newRefreshToken = tokenService.CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await userManager.UpdateAsync(user);

            return new()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken
            };
        }
    }
}
