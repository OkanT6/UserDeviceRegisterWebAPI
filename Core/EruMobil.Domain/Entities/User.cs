using EruMobil.Domain.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Domain.Entities
{
    public class User: IEntityBase
    {
        
        public User()
        {
            Devices = new List<Device>();
            CreatedDate = DateTime.UtcNow;
            IsDeleted = false;
            //ApiKeys = new List<ApiKey>();

        }
        public int Id { get; set; } // Primary key
        

        public string BusinessIdentifier { get; set; }

        public string UserType { get; set; }

        public string? access_token_when_register { get; set; }


        public ICollection<Device> Devices { get; set; }


        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        // Yeni alan:
        //public ICollection<ApiKey> ApiKeys { get; set; } // bir kullanıcıya birden fazla key olabilir
    }
}
