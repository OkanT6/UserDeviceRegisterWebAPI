using EruMobil.Application.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Exceptions
{
    public class StudentNumberAndPasswordShouldBeMatched : BaseException
    {
        public StudentNumberAndPasswordShouldBeMatched() : base("Öğrenci numarası veya şifre yanlış.")
        {
        }
    }
}
