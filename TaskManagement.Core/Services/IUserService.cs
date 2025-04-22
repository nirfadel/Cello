using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Model;

namespace TaskManagement.Core.Services
{
    public interface IUserService
    {
        Task<User?> GetUser(int userId);
        Task<List<User>> GetAllUsers();
        Task<bool> IsUserAdmin(int userId);
    }
}
