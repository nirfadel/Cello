using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Model.Dto;

namespace TaskManagement.Core.Services
{
    public interface IProjectService
    {
        Task<ProjectDto> CreateProject(CreateProjectDto projectDto, string userId, bool isAdmin);
        Task<ProjectDto?> GetProject(int id, string userId, bool isAdmin);
        Task<PagedResponseDto<ProjectDto>> GetUserProjects(string userId, bool isAdmin, int pageNumber, int pageSize);
        Task<ProjectDto> UpdateProject(int id, UpdateProjectDto projectDto, string userId, bool isAdmin );
        Task DeleteProject(int id, string userId, bool isAdmin);
        Task<bool> UserCanAccessProject(int projectId, string userId, bool isAdmin);
    }
}
