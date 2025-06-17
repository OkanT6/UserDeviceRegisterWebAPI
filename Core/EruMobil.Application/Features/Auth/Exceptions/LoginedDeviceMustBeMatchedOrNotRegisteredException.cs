using EruMobil.Application.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Exceptions
{
    public class LoginedDeviceMustBeMatchedOrNotRegisteredException: BaseException
    {
        public LoginedDeviceMustBeMatchedOrNotRegisteredException() : base("Logined device must be matched with the account or not registered.")
        {
            //Yani bu cihaz ile girmeye çalışılan hesapla giriş yapılamaz!

            //Hata türü 
        }
    }
}
