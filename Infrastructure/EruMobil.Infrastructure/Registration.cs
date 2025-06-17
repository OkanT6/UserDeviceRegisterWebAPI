using EruMobil.Application.Interfaces.ObisisService;
using EruMobil.Infrastructure.TokenValidatorService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Infrastructure
{
    public static class Registration
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IObisisAPI, ObisisAPI>();

            services.AddSingleton<IPeyosisAPI, PeyosisAPI>();



            

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = false, // Do not validate the token's lifetime
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])),
                    ClockSkew = TimeSpan.Zero // Remove the delay of token when expired
                };
            });



            //// Redis ayarlarını ekle
            //services.Configure<RedisCacheSettings>(configuration.GetSection("RedisCacheSettings"));

            //// Redis servis implementasyonunu ekle
            //services.AddSingleton<IRedisCacheService, RedisCacheService>();

        }
    }
}
