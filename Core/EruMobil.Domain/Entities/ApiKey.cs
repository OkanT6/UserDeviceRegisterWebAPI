using EruMobil.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Domain.Entities
{
    public class ApiKey: IEntityBase
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;

        public bool IsRevoked { get; set; } = false;

        // Device ile ilişki
        public int DeviceId { get; set; }    // Cihaza bağladık

        public Device Device { get; set; }   // Navigasyon property

        public void Revoke()
        {
            IsRevoked = true;
        }




    }
}
