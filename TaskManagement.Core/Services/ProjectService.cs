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

namespace TaskManagement.Core.Services
{
    /// <summary>
    /// Service for handling project operations.
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<ProjectService> _logger;

        public ProjectService(IUnitOfWork unitOfWork,
            ILogger<ProjectService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ProjectDto?> GetProject(int id, string userId, bool isAdmin)
        {
            var project = await _unitOfWork.Projects.GetById(id);

            if (project == null)
                return null;

            if (!await UserCanAccessProject(id, userId, isAdmin))
                throw new UnauthorizedAccessException("You do not have access to this project");

            return new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }

        public async Task<PagedResponseDto<ProjectDto>> GetUserProjects(string userId,
            bool isAdmin, int pageNumber, int pageSize)
        {
            IReadOnlyList<Project> projects;
            int totalCount;

            if (isAdmin)
            {
                // Admins can see all projects
                projects = await _unitOfWork.Projects.GetPagedResponse(pageNumber, pageSize);
                totalCount = await _unitOfWork.Projects.Count();
            }
            else
            {
                // Regular users only see their own projects
                projects = await _unitOfWork.Projects.GetUserProjects(userId, pageNumber, pageSize);
                totalCount = await _unitOfWork.Projects.GetUserProjectsCount(userId);
            }

            var projectDtos = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            return new PagedResponseDto<ProjectDto>
            {
                Items = projectDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProjectDto> UpdateProject(int id, UpdateProjectDto projectDto, string userId, bool isAdmin)
        {
            var project = await _unitOfWork.Projects.GetById(id);
            if (project == null)
                throw new NotFoundException($"Project with ID {id} not found");

            if (!await UserCanAccessProject(id, userId, isAdmin))
                throw new UnauthorizedAccessException("You do not have access to this project");

            // Check if user is owner or admin
            if (project.OwnerId != userId && !isAdmin)
                throw new UnauthorizedAccessException("Only the project owner or an admin can update this project");

            project.Title = projectDto.Title;
            project.Description = projectDto.Description;
            project.UpdatedAt = DateTime.Now;

            await _unitOfWork.Projects.Update(project);
            await _unitOfWork.Complete();

            return new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }

        public async Task DeleteProject(int id, string userId, bool isAdmin)
        {
            var project = await _unitOfWork.Projects.GetById(id);

            if (project == null)
                throw new NotFoundException($"Project with ID {id} not found");

            //// Check if user is owner or admin
            var access = await UserCanAccessProject(id, userId, isAdmin);
            if (!access)
                throw new UnauthorizedAccessException("Only the project owner or an admin can delete this project");

            await _unitOfWork.Projects.Delete(project);
            await _unitOfWork.Complete();

            _logger.LogInformation("Project {ProjectId} deleted by user {UserId}", id, userId);
        }

        public async Task<bool> UserCanAccessProject(int projectId, string userId, bool isAdmin)
        {
            if (isAdmin)
                return true;

            var project = await _unitOfWork.Projects.GetById(projectId);
            return project != null && project.OwnerId == userId;
        }

        public async Task<ProjectDto> CreateProject(CreateProjectDto projectDto, string userId, bool isAdmin)
        {
            _logger.LogInformation("Creating new project for user {UserId}", userId);
            ProjectDto projectDto1 = new ProjectDto();
            Project project = new Project
            {
                Title = projectDto.Title,
                Description = projectDto.Description,
                OwnerId = userId
            };
            try
            {
               
                await _unitOfWork.Projects.Add(project);
                await _unitOfWork.Complete();
                projectDto1.Id = project.Id;
                projectDto1.Title = project.Title;
                projectDto1.Description = project.Description;
            }
            catch (Exception ex)
            {
                var message = $"Error while creating project: {ex.Message}";
                _logger.LogError(message, ex);
                throw new System.ApplicationException(message, ex);
            }

            return projectDto1;
        }
    }
}
