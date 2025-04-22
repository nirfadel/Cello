using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DB;
using TaskManagement.Core.Model;

namespace TaskManagement.Core.Repository
{
    /// <summary>
    /// Generic repository for CRUD operations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {

        private readonly TaskManageDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(TaskManageDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<T?> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetPagedResponse(int pageNumber, int pageSize)
        {
            return await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> Get(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<int> Count(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
        }

        public async Task<T> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task Update(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task Delete(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }
    }
}
