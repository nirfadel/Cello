using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Model;

namespace TaskManagement.Core.Repository
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<IReadOnlyList<Project>> GetUserProjects(string userId, int pageNumber, int pageSize);
        Task<int> GetUserProjectsCount(string userId);
    }
}
