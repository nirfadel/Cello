using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DB;
using TaskManagement.Core.Helper;
using TaskManagement.Core.Model;
using TaskManagement.Core.Model.Dto;
using static TaskManagement.Core.Model.Enums;

namespace TaskManagement.Core.Services
{
    /// <summary>
    /// Service for handling task operations.
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskService> _logger;
        private readonly IUserClaimsService _userClaimsService;

        public TaskService(
            IUnitOfWork unitOfWork,
            ILogger<TaskService> logger,
            IUserClaimsService userClaimsService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userClaimsService = userClaimsService;
        }

        public async Task<TaskDto> CreateTask(int projectId, CreateTaskDto taskDto, string userId)
        {
            // Check if project exists
            var project = await _unitOfWork.Projects.GetById(projectId);
            if (project == null)
                throw new NotFoundException($"Project with ID {projectId} not found");

            var task = new TaskObj
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status,
                ProjectId = projectId,
                AssignedToId = taskDto.AssignedToId
            };

            await _unitOfWork.Tasks.Add(task);
            await _unitOfWork.Complete();

            _logger.LogInformation("Task {TaskId} created for project {ProjectId} by user {UserId}",
                task.Id, projectId, userId);

            return await MapToTaskDto(task);
        }

        public async Task DeleteTask(int projectId, int taskId, string userId)
        {
            var task = await _unitOfWork.Tasks.GetById(taskId);

            if (task == null)
                throw new NotFoundException($"Task with ID {taskId} not found");

            if (task.ProjectId != projectId)
                throw new BadRequestException("Task does not belong to specified project");

            await _unitOfWork.Tasks.Delete(task);
            await _unitOfWork.Complete();

            _logger.LogInformation("Task {TaskId} deleted by user {UserId}", taskId, userId);
        }

        public async Task<PagedResponseDto<TaskDto>> GetProjectTasks(int projectId, int pageNumber, int pageSize, string userId)
        {
            var projectTasks = await _unitOfWork.Tasks.GetTasksByProjectId(projectId, pageNumber, pageSize);
            var totalCount = await _unitOfWork.Tasks.GetTasksCountByProjectId(projectId);
            var taskDtos = new List<TaskDto>();
            foreach (var task in projectTasks)
            {
                taskDtos.Add(await MapToTaskDto(task));
            }

            _logger.LogInformation("Retrieved {Count} tasks for project {ProjectId}, page {PageNumber}",
            projectTasks.Count, projectId, pageNumber);
            var pagedResponse = new PagedResponseDto<TaskDto>
            {
                Items = taskDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };

            return pagedResponse;
        }

        public async Task<TaskDto?> GetTask(int projectId, int taskId, string userId)
        {
            var task = await _unitOfWork.Tasks.GetById(taskId);

            if (task == null)
                throw new NotFoundException($"Task with ID {taskId} not found");

            return await MapToTaskDto(task);
        }

        public async Task<TaskDto> UpdateTask(int projectId, int taskId, UpdateTaskDto taskDto, string userId)
        {
            var task = await _unitOfWork.Tasks.GetById(taskId);

            if (task == null)
                throw new NotFoundException($"Task with ID {taskId} not found");

            if (task.ProjectId != projectId)
                throw new BadRequestException("Task does not belong to specified project");

            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.Status = taskDto.Status;
            task.AssignedToId = taskDto.AssignedToId;
            task.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Tasks.Update(task);
            await _unitOfWork.Complete();

            _logger.LogInformation("Task {TaskId} updated by user {UserId}", taskId, userId);

            return await MapToTaskDto(task);
        }

        private async Task<TaskDto> MapToTaskDto(TaskObj task)
        {
            string? assignedToName = null;

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                ProjectId = task.ProjectId,
                AssignedToId = task.AssignedToId,
                //AssignedToName = assignedToName,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}
