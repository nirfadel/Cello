using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TaskManagement.Core.Model.Enums;

namespace TaskManagement.Core.Model
{
    public class TaskObj : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public StatusOfTask Status { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public string? AssignedToId { get; set; } 
       
    }
    
}
