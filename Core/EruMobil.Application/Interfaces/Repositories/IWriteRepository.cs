using EruMobil.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Application.Interfaces.Repositories
{
    public interface IWriteRepository<T> where T : class, IEntityBase, new()
    {
        Task AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        Task<T> UpdateAsync(T entity);
        Task HardDeleteAsync(T entity);

        Task HardDeleteRangeAsync(IList<T> entities);


        //Task SoftDeleteAsync(T entity);





    }
}
