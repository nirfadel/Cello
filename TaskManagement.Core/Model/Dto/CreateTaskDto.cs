using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TaskManagement.Core.Model.Enums;

namespace TaskManagement.Core.Model.Dto
{
    public class CreateTaskDto
    {
        [Required]
        //[StringLength(100, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public StatusOfTask Status { get; set; } = StatusOfTask.ToDo;

        public string? AssignedToId { get; set; }
    }
}
