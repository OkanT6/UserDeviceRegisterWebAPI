using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Interfaces
{
    public interface IMersisAPI
    {
        Task<bool> IsTcIsValid(string TCNo);
    }
}
