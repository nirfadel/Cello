using Amazon.CognitoIdentityProvider.Model;
using Amazon.CognitoIdentityProvider;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Authentication;
using TaskManagement.Core.Model.Dto;
using Microsoft.Extensions.Logging;
using Amazon;
using TaskManagement.Core.Helper;

namespace TaskManagement.Core.Services
{
    /// <summary>
    /// Service for handling authentication with AWS Cognito.
    /// </summary>
    public class CognitoAuthService : IAuthService
    {
        private readonly IAmazonCognitoIdentityProvider _cognitoProvider;
        private readonly CognitoSettings _cognitoSettings;
        private readonly ILogger<ProjectService> _logger;

        public CognitoAuthService(
            IAmazonCognitoIdentityProvider cognitoProvider,
            IOptions<CognitoSettings> cognitoSettings,
            ILogger<ProjectService> logger)
        {
            _cognitoProvider = cognitoProvider;
            _cognitoSettings = cognitoSettings.Value;
            _logger = logger;
            Console.WriteLine($"Region: {_cognitoSettings.Region}, UserPoolId: {_cognitoSettings.UserPoolId}");

        }

        public async Task<AuthResponseDto> SignInAsync(AuthDto request)
        {
            var authRequest = new AdminInitiateAuthRequest
            {
                UserPoolId = _cognitoSettings.UserPoolId,
                ClientId = _cognitoSettings.AppClientId,
                AuthFlow = AuthFlowType.ADMIN_USER_PASSWORD_AUTH
            };

            var provider = new AmazonCognitoIdentityProviderClient(RegionEndpoint.USEast2);

            string clientId = _cognitoSettings.AppClientId;
            string clientSecret = _cognitoSettings.AppClientSecret;

            // Calculate the SECRET_HASH
            string secretHash = AWSHelper.CalculateSecretHash(request.Username, clientId, clientSecret);

            authRequest.AuthParameters.Add("USERNAME", request.Username);
            authRequest.AuthParameters.Add("PASSWORD", request.Password);
            authRequest.AuthParameters.Add("SECRET_HASH", secretHash);
          

            var authResponse = await _cognitoProvider.AdminInitiateAuthAsync(authRequest);

            return new AuthResponseDto
            {
                AccessToken = authResponse.AuthenticationResult.AccessToken,
                IdToken = authResponse.AuthenticationResult.IdToken,
                RefreshToken = authResponse.AuthenticationResult.RefreshToken,
                ExpiresIn = (int)authResponse.AuthenticationResult.ExpiresIn
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var refreshRequest = new AdminInitiateAuthRequest
            {
                UserPoolId = _cognitoSettings.UserPoolId,
                ClientId = _cognitoSettings.AppClientId,
                AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH
            };

            refreshRequest.AuthParameters.Add("REFRESH_TOKEN", refreshToken);

            var authResponse = await _cognitoProvider.AdminInitiateAuthAsync(refreshRequest);

            return new AuthResponseDto
            {
                AccessToken = authResponse.AuthenticationResult.AccessToken,
                IdToken = authResponse.AuthenticationResult.IdToken,
                RefreshToken = authResponse.AuthenticationResult.RefreshToken ?? refreshToken,
                ExpiresIn = (int)authResponse.AuthenticationResult.ExpiresIn
            };
        }

        // Step 1: Initiate forgot password flow
        public async Task ForgotPasswordAsync(string username)
        {
            var request = new ForgotPasswordRequest
            {
                ClientId = _cognitoSettings.AppClientId,
                Username = username
            };

            await _cognitoProvider.ForgotPasswordAsync(request);
            // This sends a verification code to the user's email or phone
        }

        // Step 2: Complete the forgot password process with the verification code
        public async Task ConfirmForgotPasswordAsync(string username, string confirmationCode, string newPassword)
        {
            var request = new ConfirmForgotPasswordRequest
            {
                ClientId = _cognitoSettings.AppClientId,
                Username = username,
                ConfirmationCode = confirmationCode,
                Password = newPassword
            };

            await _cognitoProvider.ConfirmForgotPasswordAsync(request);
        }

        public async Task ChangeUserPasswordAsync(string username, string newPassword)
        {
            var request = new AdminSetUserPasswordRequest
            {
                UserPoolId = _cognitoSettings.UserPoolId,
                Username = username,
                Password = newPassword,
                Permanent = true // Set to true for a permanent password, false for temporary
            };

            await _cognitoProvider.AdminSetUserPasswordAsync(request);
            _logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, $"Password changed for user {username}");
        }
    }

}
