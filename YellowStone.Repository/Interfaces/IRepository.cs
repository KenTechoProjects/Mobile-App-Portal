using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace YellowStone.Repository.Interfaces
{ 
    public interface IRepository<T> where T : class
    {
       // Task<IEnumerable<T>> GetAll();
        IQueryable<T> Find(Expression<Func<T, bool>> Predicate);
        Task<T> GetByID(long id);
        Task<bool> Any(Expression<Func<T, bool>> Predicate);
        Task<T> Create(T Entity);
        Task<T> Update(T Entity);
        Task Delete(T Entity);
        Task<int> Count(Expression<Func<T, bool>> Predicate);
        Task<T> Single(Expression<Func<T, bool>> Predicate);
        IQueryable<T> FindWhere(Expression<Func<T, bool>> Predicate);
    }
}
