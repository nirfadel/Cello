using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TaskManagement.Core.Model.Enums;
using TaskManagement.Core.Model;
using TaskManagement.Core.Services;

namespace TaskManagement.Core.Authentication
{
    public class CognitoUserService : IUserService
    {
        private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
        private readonly CognitoSettings _cognitoSettings;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CognitoUserService> _logger;

        public CognitoUserService(
            IOptions<CognitoSettings> cognitoSettings,
            IMemoryCache cache,
            ILogger<CognitoUserService> logger)
        {
            _cognitoSettings = cognitoSettings.Value;
            _cache = cache;
            _logger = logger;

            var region = RegionEndpoint.GetBySystemName(_cognitoSettings.Region);
            _cognitoClient = new AmazonCognitoIdentityProviderClient(region);
        }

        public async Task<User?> GetUser(int userId)
        {
            // Check cache first
            string cacheKey = $"user_{userId}";
            if (_cache.TryGetValue(cacheKey, out User? cachedUser))
            {
                return cachedUser;
            }

            try
            {
                var request = new AdminGetUserRequest
                {
                    UserPoolId = _cognitoSettings.UserPoolId,
                    Username = userId.ToString()
                };

                var response = await _cognitoClient.AdminGetUserAsync(request);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    return null;
                }

                var user = new User
                {
                    Id = userId,
                    Email = GetUserAttribute(response.UserAttributes, "email"),
                    Name = GetUserAttribute(response.UserAttributes, "name") ??
                           GetUserAttribute(response.UserAttributes, "given_name") ?? userId.ToString(),
                    Role = GetUserRole(response.UserAttributes)
                };

                // Cache the user for 5 minutes
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));

                return user;
            }
            catch (UserNotFoundException)
            {
                _logger.LogWarning("User {UserId} not found in Cognito", userId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId} from Cognito", userId);
                throw;
            }
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = new List<User>();
            string? paginationToken = null;

            try
            {
                do
                {
                    var request = new ListUsersRequest
                    {
                        UserPoolId = _cognitoSettings.UserPoolId,
                        Limit = 60,
                        PaginationToken = paginationToken
                    };

                    var response = await _cognitoClient.ListUsersAsync(request);
                    paginationToken = response.PaginationToken;

                    foreach (var userType in response.Users)
                    {
                        var user = new User
                        {
                            Email = GetUserAttribute(userType.Attributes, "email"),
                            Name = GetUserAttribute(userType.Attributes, "name") ??
                                   GetUserAttribute(userType.Attributes, "given_name") ?? userType.Username,
                            Role = GetUserRole(userType.Attributes)
                        };

                        users.Add(user);
                    }
                }
                while (!string.IsNullOrEmpty(paginationToken));

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users from Cognito");
                throw;
            }
        }

        public async Task<bool> IsUserAdmin(int userId)
        {
            var user = await GetUser(userId);
            return user?.Role == UserRole.Admin;
        }

        private string? GetUserAttribute(List<AttributeType> attributes, string attributeName)
        {
            return attributes
                .FirstOrDefault(a => a.Name == attributeName)?.Value;
        }

        private UserRole GetUserRole(List<AttributeType> attributes)
        {
            // Look for custom:role attribute first
            var roleAttribute = attributes.FirstOrDefault(a => a.Name == "custom:role")?.Value;

            if (!string.IsNullOrEmpty(roleAttribute))
            {
                if (roleAttribute.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    return UserRole.Admin;
            }

            // Check if user is in admin group via cognito:groups
            var groupsAttribute = attributes.FirstOrDefault(a => a.Name == "cognito:groups")?.Value;
            if (!string.IsNullOrEmpty(groupsAttribute) &&
                groupsAttribute.Contains("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return UserRole.Admin;
            }

            return UserRole.User;
        }
    }
}
