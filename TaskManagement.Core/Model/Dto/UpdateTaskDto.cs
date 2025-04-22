using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TaskManagement.Core.Model.Enums;

namespace TaskManagement.Core.Model.Dto
{
    public class UpdateTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public StatusOfTask Status { get; set; }
        public string? AssignedToId { get; set; }
    }
}
