using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Model.Dto;

namespace TaskManagement.Core.Services
{
    public interface ITaskService
    {
        Task<TaskDto> CreateTask(int projectId, CreateTaskDto taskDto, string userId);
        Task<TaskDto?> GetTask(int projectId, int taskId, string userId);
        Task<PagedResponseDto<TaskDto>> GetProjectTasks(int projectId, int pageNumber, int pageSize, string userId);
        Task<TaskDto> UpdateTask(int projectId, int taskId, UpdateTaskDto taskDto, string userId);
        Task DeleteTask(int projectId, int taskId, string userId);
    }
}
