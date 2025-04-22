using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Model;
using TaskManagement.Core.Repository;

namespace TaskManagement.Core.DB
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskManageDbContext _dbContext;
        public IProjectRepository Projects { get; }
        public ITaskRepository Tasks { get; }
        private bool _disposed = false;
        public UnitOfWork(TaskManageDbContext dbContext,
            IProjectRepository projectRepository, ITaskRepository taskRepository)
        {
            _dbContext = dbContext;
            Projects = projectRepository;
            Tasks = taskRepository;
        }
        public async Task<int> Complete()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
