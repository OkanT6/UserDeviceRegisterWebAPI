using EruMobil.Application.Interfaces.Repositories;
using EruMobil.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Persistence.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class, IEntityBase, new()
    {
        private readonly DbContext _context;
        public WriteRepository(DbContext context)
        {
            _context = context;
        }

        private DbSet<T> Table { get => _context.Set<T>(); }

        public async Task AddAsync(T entity)
        {
            await Table.AddAsync(entity);
        }

        public async Task AddRangeAsync(ICollection<T> entities)
        {
            await Table.AddRangeAsync(entities);
        }
        public async Task<T> UpdateAsync(T entity)
        {
            await Task.Run(() => Table.Update(entity));
            return entity;
        }

        public async Task HardDeleteAsync(T entity)
        {
            await Task.Run(() => Table.Remove(entity));
        }

        public async Task HardDeleteRangeAsync(IList<T> entities)
        {
            await Task.Run(() => Table.RemoveRange(entities));
        }

        //public async Task SoftDeleteAsync(T entity)
        //{
        //    await Task.Run(() => Table.Update(entity));

        //}


    }
}
