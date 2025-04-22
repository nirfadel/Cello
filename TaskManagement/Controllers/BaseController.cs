using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskManagement.Controllers
{
    [ApiController]
   // [Authorize]
    //[Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected string GetUserId()
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID not found in claims");
            }
            return userId;
        }

        protected string GetUserRole()
        {
            var role = User.FindFirst("role")?.Value;
            if (string.IsNullOrEmpty(role))
            {
                throw new UnauthorizedAccessException("User role not found in claims");
            }
            return role;
        }

        protected bool IsAdmin()
        {
            var role = User.FindFirst("scope")?.Value;
            if (string.IsNullOrEmpty(role))
            {
                throw new UnauthorizedAccessException("User role not found in claims");
            }
            return role.Contains("admin") ? true : false;
        }


    }
}
