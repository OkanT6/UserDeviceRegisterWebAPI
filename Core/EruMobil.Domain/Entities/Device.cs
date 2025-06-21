using EruMobil.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Domain.Entities
{
    public class Device:IEntityBase
    {
        public Guid Id { get; set; } // Primary key

        public string DeviceName { get; set; }
        public string UniqueDeviceIdentifier { get; set; }

        public string Platform { get; set; }

        public string FcmToken { get; set; }

        public string AppVersion { get; set; }

        public Guid UserId { get; set; } // Foreign Key

        public User User { get; set; }

        public bool NotificationsIsActive { get; set; }


    }

    
}
