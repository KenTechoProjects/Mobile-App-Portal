using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Repository.Interfaces;

namespace YellowStone.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        protected FBNMDashboardContext _context;
        public Repository(FBNMDashboardContext context)
        {
            this._context = context;
        }

        protected Task SaveChanges() => _context.SaveChangesAsync();
        //protected Task SaveChanges() {
            
        //    using(var trans= )
            
            
        //    _context.SaveChangesAsync();
        
        
        //}

        public async Task<bool> Any(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(Predicate).AnyAsync();
        }

        public async Task<int> Count(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(Predicate).CountAsync();
        }

        public async Task<T> Create(T Entity)
        {
          
            await _context.Set<T>().AddAsync(Entity);
            await SaveChanges();

            return Entity;
        }

        public async Task<T> Update(T Entity)
        {
            _context.Entry(Entity).State = EntityState.Modified;
            await SaveChanges();

            return Entity;
        }

        public async Task Delete(T Entity)
        {
            _context.Set<T>().Remove(Entity);
            await SaveChanges();
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> Predicate)
        {
            return _context.Set<T>().AsNoTracking().Where(Predicate).AsQueryable();
        }

        public IQueryable<T> FindWhere(Expression<Func<T, bool>> Predicate)
        {
            return _context.Set<T>().AsNoTracking().Where(Predicate);
        }

        public async Task<T> Single(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(Predicate);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByID(long Id)
        {
            return await _context.Set<T>().FindAsync(Id);
        }
    }

}
