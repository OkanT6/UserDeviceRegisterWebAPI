using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Features.Auth.Commands.Register
{
    public class BusinessIdentitfierGenerator
    {
        public string CreateStudentNumber()
        {
            Random random = new Random();
            int number = random.Next(1030510000, 1030519999); // Generates a random number between 100000000 and 999999999
            return number.ToString();
        }

        public string CreateStaffNumber()
        {
            Random random = new Random();
            int number = random.Next(1990510000, 1990519999); // Generates a random number between 100000000 and 999999999
            return number.ToString();
        }

        public string CreateDefaultUserNumber()
        {
            Random random = new Random();
            int number = random.Next(0990510000, 0990519999); // Generates a random number between 100000000 and 999999999
            return number.ToString();
        }

        


    }
}
