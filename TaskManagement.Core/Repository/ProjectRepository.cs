using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DB;
using TaskManagement.Core.Model;

namespace TaskManagement.Core.Repository
{
    /// <summary>
    /// Project repository for CRUD operations on Project.
    /// </summary>
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly TaskManageDbContext _context; 

        public ProjectRepository(TaskManageDbContext context) : base(context) 
        {
            _context = context; 
        }

        public async Task<IReadOnlyList<Project>> GetUserProjects(string userId, int pageNumber, int pageSize)
        {
            List<Project> result = new List<Project>();
            try
            {
                result = await _context.Projects
               .Where(p => p.OwnerId == userId)
               .OrderByDescending(p => p.CreatedAt)
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while getting projects by user.", ex);
            }
            return result;
        }

        public async Task<int> GetUserProjectsCount(string userId)
        {
            return await _context.Projects
                      .CountAsync(p => p.OwnerId == userId);
        }
    }
}
