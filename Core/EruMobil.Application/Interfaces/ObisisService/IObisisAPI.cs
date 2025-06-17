using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Interfaces.ObisisService
{
    public interface IObisisAPI
    {
        Task<bool> IsStudentTokenValid(string accessTokenObisis);

    }
}
