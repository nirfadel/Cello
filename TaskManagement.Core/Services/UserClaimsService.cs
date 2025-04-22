using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Model;

namespace TaskManagement.Core.Services
{
    /// <summary>
    /// Service for handling user claims.
    /// </summary>
    public class UserClaimsService : IUserClaimsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserClaimsService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetUserId()
        {
           return _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value;
        }

        public bool IsAdmin()
        {
            var role = _httpContextAccessor.HttpContext?.User?.FindFirst("scope")?.Value;
            if (string.IsNullOrEmpty(role))
            {
                throw new UnauthorizedAccessException("User role not found in claims");
            }
            return role.Contains("admin") ? true : false;
        }
    }
}
