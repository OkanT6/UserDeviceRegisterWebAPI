using EruMobil.Application.Interfaces.Repositories;
using EruMobil.Application.Interfaces.UnitOfWorks;
using EruMobil.Domain.Entities;
using EruMobil.Persistence.Context;
using EruMobil.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EruMobil.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EruMobil.Persistence.UnitOfWorks;

namespace EruMobil.Persistence
{
    public static class Registration
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
            

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //services.AddIdentityCore<User>(opt =>
            //{
            //    opt.User.RequireUniqueEmail = true;
            //    opt.SignIn.RequireConfirmedAccount = false;
            //    opt.Password.RequireDigit = false;
            //    opt.Password.RequiredLength = 6;
            //    opt.Password.RequireNonAlphanumeric = false;
            //    opt.Password.RequireUppercase = false;
            //    opt.Password.RequireLowercase = false;
            //})
            //    .AddRoles<Role>()
            //    .AddEntityFrameworkStores<AppDbContext>();

        }
    }
}
