using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Authentication;
using TaskManagement.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
namespace TaskManagement.Core.Extensions
{

    /// <summary>
    /// Extension methods for setting up AWS Cognito authentication in ASP.NET Core.
    /// </summary>
    public static class CognitoAuthenticationExtensions
    {
        public static IServiceCollection AddCognitoAuthentication(
        this IServiceCollection services,
        string region,
        string userPoolId,
        string appClientId)
        {
            // Configure JWT Bearer Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = false, 
                    //ValidateLifetime = true,
                    ValidIssuer = options.Authority,
                    //ValidAudience = appClientId
                };
                // Add these event handlers for debugging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");

                        // Log the exception type and message
                        Console.WriteLine($"Auth Failed Exception: {context.Exception.GetType().Name}");
                        Console.WriteLine($"Auth Failed Message: {context.Exception.Message}");

                        // If there's an inner exception, log that too
                        if (context.Exception.InnerException != null)
                        {
                            Console.WriteLine($"Inner Exception: {context.Exception.InnerException.GetType().Name}");
                            Console.WriteLine($"Inner Exception Message: {context.Exception.InnerException.Message}");
                        }

                        // Log the current token if available
                        if (context.Request.Headers.ContainsKey("Authorization"))
                        {
                            var authHeader = context.Request.Headers["Authorization"].ToString();
                            if (authHeader.StartsWith("Bearer "))
                            {
                                var token = authHeader.Substring("Bearer ".Length).Trim();
                                Console.WriteLine($"Token starts with: {token.Substring(0, Math.Min(20, token.Length))}...");
                                Console.WriteLine($"Token parts: {token.Split('.').Length}");
                            }
                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token was validated successfully");
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        Console.WriteLine($"Token received: {context.Token?.Substring(0, 10)}...");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"Challenge issued: {context.AuthenticateFailure?.Message}");
                        return Task.CompletedTask;
                    }
                };

            });

            // Add authorization policies if needed
            services.AddAuthorization(options =>
            {
                // Default policy - requires authenticated user
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // Optional: Add custom policies based on Cognito groups or claims
                options.AddPolicy("AdminsOnly", policy => policy
                    .RequireClaim("scope", "aws.cognito.signin.user.admin"));
            });

            return services;
        }
    }
}
