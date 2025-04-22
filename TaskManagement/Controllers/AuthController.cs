using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Model.Dto;
using TaskManagement.Core.Services;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signin")]
        public async Task<ActionResult<AuthResponseDto>> SignIn([FromBody] AuthDto request)
        {
            try
            {
                var response = await _authService.SignInAsync(request);
                return Ok(response);
            }
            catch (NotAuthorizedException ex)
            {
                return Unauthorized(new { message = "Invalid username or password", exMessage = ex.Message });
            }
            catch (UserNotFoundException)
            {
                return NotFound(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during sign in", error = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(refreshToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while refreshing token", error = ex.Message });
            }
        }

        [HttpPost("forgot-password-admin")]
        public async Task<ActionResult<AuthResponseDto>> ChangeUserPassword(string userName, string newPassword)
        {
            try
            {
                await _authService.ChangeUserPasswordAsync(userName, newPassword);
                return Ok();
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { message = $"An error occurred while changing password for user {userName}", error = ex.Message });
            }
        }

    }
}
