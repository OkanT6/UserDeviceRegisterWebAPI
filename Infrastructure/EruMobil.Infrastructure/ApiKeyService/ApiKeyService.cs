using EruMobil.Application.Interfaces.ApiKey;
using EruMobil.Application.Interfaces.Repositories;
using EruMobil.Application.Interfaces.UnitOfWorks;
using EruMobil.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Infrastructure.ApiKeyService
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IUnitOfWork unitOfWork;


        public ApiKeyService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateKeyAsync(int deviceId, TimeSpan validityDuration)
        {
            var key = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + "-" + Guid.NewGuid();

            var apiKey = new ApiKey
            {
                DeviceId = deviceId,
                Key = key,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(validityDuration)
            };

            await unitOfWork.GetWriteRepository<ApiKey>().AddAsync(apiKey);
            await unitOfWork.SaveAsync();

            return apiKey.Key;
        }

        public async Task<bool> ValidateKeyAsync(string apiKey)
        {
            var key = await unitOfWork.GetReadRepository<ApiKey>()
                .GetAsync(x => x.Key == apiKey);

            return key != null && key.IsActive;
        }

        public async Task RevokeKeyAsync(string apiKey)
        {
            var key = await unitOfWork.GetReadRepository<ApiKey>()
                .GetAsync(x => x.Key == apiKey,enableTracking:true);

            if (key == null)
                return;

            key.IsRevoked = true;
            await unitOfWork.SaveAsync();
        }
    }

}
