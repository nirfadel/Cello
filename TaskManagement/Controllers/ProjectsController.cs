using Amazon.Auth.AccessControlPolicy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Model;
using TaskManagement.Core.Model.Dto;
using TaskManagement.Core.Services;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : BaseApiController
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(
            IProjectService projectService,
            ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<ProjectDto>>> GetProjects(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
                return BadRequest("Page number must be greater than 0");

            if (pageSize < 1 || pageSize > 100)
                return BadRequest("Page size must be between 1 and 100");

            var userId = GetUserId();
            var isAdmin = IsAdmin();
            var result = await _projectService.GetUserProjects(userId, isAdmin, pageNumber, pageSize);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var userId = GetUserId();
            var isAdmin = IsAdmin();
            var project = await _projectService.GetProject(id, userId, isAdmin);
            if (project == null)
                return NotFound();
            return Ok(project);
        }
        [Authorize(Policy = "AdminsOnly")]
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Post([FromBody] CreateProjectDto projectDto)
        {
            if (projectDto == null)
                return BadRequest("Project data is required");
            var userId = GetUserId();
            var isAdmin = IsAdmin();
            var project = await _projectService.CreateProject(projectDto, userId, isAdmin);
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }
        [Authorize(Policy = "AdminsOnly")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProjectDto>> Put(int id, [FromBody] UpdateProjectDto projectDto)
        {
            if (projectDto == null)
                return BadRequest("Project data is required");
            var userId = GetUserId();
            var isAdmin = IsAdmin();
            var project = await _projectService.UpdateProject(id, projectDto, userId, isAdmin);
            if (project == null)
                return NotFound();
            return Ok(project);
        }

        [Authorize(Policy = "AdminsOnly")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var isAdmin = IsAdmin();
            try
            {
                await _projectService.DeleteProject(id, userId, isAdmin);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(ex.Message);
            }
        }
    }
}
