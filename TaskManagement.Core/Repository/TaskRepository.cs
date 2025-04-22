using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DB;
using TaskManagement.Core.Helper;
using TaskManagement.Core.Model;

namespace TaskManagement.Core.Repository
{
    /// <summary>
    /// Task repository for CRUD operations on TaskObj.
    /// </summary>
    public class TaskRepository : Repository<TaskObj>, ITaskRepository
    {
        private readonly TaskManageDbContext _context;
        public TaskRepository(TaskManageDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<TaskObj>> GetTasksByProjectId(int projectId, int pageNumber, int pageSize)
        {
            List<TaskObj> result = new List<TaskObj>();
            try
            {
                result = await _context.Tasks
               .Where(t => t.ProjectId == projectId)
               .OrderByDescending(t => t.CreatedAt)
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException("Error while getting tasks by project.",ex);
            }
         return result;
        }

        public async Task<int> GetTasksCountByProjectId(int projectId)
        {
            int count = 0;
            try
            {
              count = await  _context.Tasks
              .CountAsync(t => t.ProjectId == projectId);
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException("Error while getting count of tasks by project.", ex);
            }
            return count;
        }
    }
}
