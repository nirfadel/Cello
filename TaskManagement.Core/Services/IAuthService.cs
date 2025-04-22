using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Model.Dto;

namespace TaskManagement.Core.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> SignInAsync(AuthDto request);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task ChangeUserPasswordAsync(string username, string newPassword);
    }
}
