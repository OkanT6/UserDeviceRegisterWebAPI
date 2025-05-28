using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Domain.Common
{
    public class EntityBase: IEntityBase
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        // ➡️ Default constructor
        public EntityBase()
        {
            CreatedDate = DateTime.Now;
            IsDeleted = false;
            //Console.WriteLine($"[EntityBase] - CreatedDate ayarlandı: {CreatedDate}, IsDeleted: {IsDeleted}");
        }

        // ➡️ ID ile başlatmak istersek bu constructor devreye girer
        public EntityBase(int id) : this()
        {
            Id = id;
            //Console.WriteLine($"[EntityBase] - ID ayarlandı: {Id}");
        }
    }
}
