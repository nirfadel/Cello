using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Repository;

namespace TaskManagement.Core.DB
{
    public interface IUnitOfWork
    {
        IProjectRepository Projects { get; }
        ITaskRepository Tasks { get; }
        Task<int> Complete();
    }
}
