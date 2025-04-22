using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Model;

namespace TaskManagement.Core.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmail(string email);
        Task<IReadOnlyList<User>> GetUsersByRole(string role, int pageNumber, int pageSize);
        Task<int> GetUsersByRoleCount(string role);
    }
    
}
