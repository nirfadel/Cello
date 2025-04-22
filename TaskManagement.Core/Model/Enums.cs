using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Model
{
    public class Enums
    {
        public enum StatusOfTask
        {
            ToDo,
            InProgress,
            Done
        }

        public enum UserRole
        {
            User,
            Admin
        }

        public enum ErrorSeverity
        {
            Low,
            Medium,
            High,
            Critical
        }
    }
}
