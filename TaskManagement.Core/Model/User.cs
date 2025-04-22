using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TaskManagement.Core.Model.Enums;

namespace TaskManagement.Core.Model
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
