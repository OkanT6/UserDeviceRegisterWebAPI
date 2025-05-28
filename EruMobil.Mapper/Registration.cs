using EruMobil.Application.Interfaces.AutoMapper;
using EruMobil.Mapper.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Mapper
{
    public static class Registration
    {
        public static void AddCustomMapper(this IServiceCollection services)
        {
            services.AddSingleton<IMapper, EruMobil.Mapper.AutoMapper.Mapper>();
        }
    }
}
