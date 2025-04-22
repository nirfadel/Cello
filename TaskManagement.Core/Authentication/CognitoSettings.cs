using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Authentication
{
    public class CognitoSettings
    {
        public string Region { get; set; } = string.Empty;
        public string UserPoolId { get; set; } = string.Empty;
        public string AppClientId { get; set; } = string.Empty;
        public string AppClientSecret { get; set; } = string.Empty;
        public string Authority { get; set; } = string.Empty;
    }
}
