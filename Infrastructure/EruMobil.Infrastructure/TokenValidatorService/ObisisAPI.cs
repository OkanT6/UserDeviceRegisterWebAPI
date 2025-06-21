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

            // Bundan sonra "Bearer " trim edilecek ve token elde edilecek!
            string token = accessTokenObisis.Substring("Bearer ".Length).Trim();

            // Token boşsa yine geçersiz sayılabilir
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(false);
            }

            // Token işleme devam edilebilir
            return Task.FromResult(true);
        }
    }
}
