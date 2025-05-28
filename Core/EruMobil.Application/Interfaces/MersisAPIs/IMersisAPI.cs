using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Interfaces.MersisAPIs
{
    public interface IMersisAPI
    {
        Task IsTcIsValidAsync(string TCNo);
    }
}
