using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Model;

namespace TaskManagement.Core.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetById(int id);
        Task<IReadOnlyList<T>> GetAll();
        Task<IReadOnlyList<T>> GetPagedResponse(int pageNumber, int pageSize);
        Task<IReadOnlyList<T>> Get(Expression<Func<T, bool>> predicate);
        Task<int> Count(Expression<Func<T, bool>>? predicate = null);
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
