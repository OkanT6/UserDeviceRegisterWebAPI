using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Bases
{
    public class BaseException : ApplicationException
    {
        public BaseException() { }
        public BaseException(string message) : base(message) { }
        //public BaseExceptions(string message, Exception innerException) : base(message, innerException)
        //{
        //}
    }
}
