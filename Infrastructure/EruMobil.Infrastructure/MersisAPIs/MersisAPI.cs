using EruMobil.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Infrastructure.MersisAPIs
{
    public class MersisAPI : IMersisAPI
    {
        public Task<bool> IsTcIsValid(string TCNo)
        {
            if(string.IsNullOrEmpty(TCNo) || TCNo.Length != 11 || !long.TryParse(TCNo, out _))
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
    }
}
