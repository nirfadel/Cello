using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Model;

namespace TaskManagement.Core.Repository
{
    public interface ITaskRepository : IRepository<TaskObj>
    {
        Task<IReadOnlyList<TaskObj>> GetTasksByProjectId(int projectId, int pageNumber, int pageSize);
        Task<int> GetTasksCountByProjectId(int projectId);
    }
}
