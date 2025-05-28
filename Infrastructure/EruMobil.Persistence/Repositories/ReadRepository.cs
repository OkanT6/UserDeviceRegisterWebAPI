using EruMobil.Application.Interfaces.Repositories;
using EruMobil.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EruMobil.Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : class, IEntityBase, new()
    {
        private readonly DbContext _context;
        public ReadRepository(DbContext context)
        {
            _context = context;
        }

        private DbSet<T> Table { get => _context.Set<T>(); }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool enableTracking = false)
        {
            IQueryable<T> query = Table;
            if (!(enableTracking)) query = query.AsNoTracking();
            if (include is not null) query = include(query);
            if (predicate is not null) query = query.Where(predicate);
            if (orderBy is not null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();

        }




        public async Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool enableTracking = false, int currentPage = 1, int pageSize = 3)
        {
            IQueryable<T> query = Table;
            if (!(enableTracking)) query = query.AsNoTracking();
            if (include is not null) query = include(query);
            if (predicate is not null) query = query.Where(predicate);
            if (orderBy is not null)
                return await orderBy(query).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            return await query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool enableTracking = false, int currentPage = 1, int pageSize = 3)
        {
            IQueryable<T> query = Table;
            if (!(enableTracking)) query = query.AsNoTracking();
            if (include is not null) query = include(query);
            // 🔄 Filtreyi doğrudan sorguya uyguluyoruz
            query = query.Where(predicate);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            Table.AsNoTracking();
            if (predicate is not null) Table.Where(predicate);

            return await Table.CountAsync();
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate, bool enableTracking = false)
        {
            if (!enableTracking) Table.AsNoTracking();
            return Table.Where(predicate);

        }
    }
}
