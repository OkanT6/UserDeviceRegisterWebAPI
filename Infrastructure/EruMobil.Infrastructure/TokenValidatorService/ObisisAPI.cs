using EruMobil.Application.Interfaces.ObisisService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Infrastructure.TokenValidatorService
{
    public class ObisisAPI:IObisisAPI
    {
        public Task<bool> IsStudentTokenValid(string accessTokenObisis)
        {
            if (string.IsNullOrEmpty(accessTokenObisis))
            {
                return Task.FromResult(false);
            }

            if (!accessTokenObisis.StartsWith("Bearer "))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
