using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Interfaces.ApiKey
{
    public interface IApiKeyService
    {
        Task<string> GenerateKeyAsync(int userId, TimeSpan validityDuration);
        Task<bool> ValidateKeyAsync(string apiKey);
        Task RevokeKeyAsync(string apiKey);
    }
}
