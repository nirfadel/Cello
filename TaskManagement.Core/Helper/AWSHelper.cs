using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Helper
{
    /// <summary>
    /// Helper class for AWS Cognito authentication.
    /// </summary>
    public class AWSHelper
    {
        /// <summary>
        /// Calculates the secret hash for AWS Cognito authentication.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public static string CalculateSecretHash(string username, string clientId, string clientSecret)
        {
            // Prepare the message (username + clientId)
            string message = username + clientId;

            // Convert the secret key to bytes
            byte[] keyBytes = Encoding.UTF8.GetBytes(clientSecret);

            // Convert the message to bytes
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // Calculate the HMAC SHA-256 hash
            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(messageBytes);

                // Convert to Base64
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
