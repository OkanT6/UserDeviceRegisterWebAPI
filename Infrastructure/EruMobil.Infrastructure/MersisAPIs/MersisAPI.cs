using EruMobil.Application.Features.Auth.Exceptions;
using EruMobil.Application.Interfaces.MersisAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Infrastructure.MersisAPIs
{
    public class MersisAPI : IMersisAPI
    {
        public Task IsTcIsValidAsync(string TCNo)
        {
            if(string.IsNullOrEmpty(TCNo) || TCNo.Length != 11 || !long.TryParse(TCNo, out _))
            {
                throw new TCNoIsNotValidException();
            }
            return Task.CompletedTask;
        }
    }
}
