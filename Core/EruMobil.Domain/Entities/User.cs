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
        }
        public int Id { get; set; } // Primary key
        //public string? AccessToken { get; set; }

        //public string? StudentNumber { get; set; }

        public string BusinessIdentifier { get; set; }

        public string UserType { get; set; }

        public string? access_token_when_register { get; set; }

        //public string TCNo { get; set; }

        public ICollection<Device> Devices { get; set; }

        //public string FullName { get; set; }

        //public string? RefreshToken { get; set; }

        //public DateTime? RefreshTokenEndDate { get; set; }

        public bool NotificationsIsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
