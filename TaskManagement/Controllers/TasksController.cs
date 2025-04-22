using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Model.Dto;
using TaskManagement.Core.Services;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : BaseApiController 
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;
        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("projects/{projectId}/tasks")]
        public async Task<ActionResult<PagedResponseDto<TaskDto>>> GetTasks(
           int projectId,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
                return BadRequest("Page number must be greater than 0");

            if (pageSize < 1 || pageSize > 100)
                return BadRequest("Page size must be between 1 and 100");

            var userId = GetUserId();

            try
            {
                var result = await _taskService.GetProjectTasks(projectId, pageNumber, pageSize, userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [Authorize]
        [HttpGet("projects/{projectId}/tasks/{taskId}")]
        public async Task<ActionResult<TaskDto>> GetTask(int projectId, int taskId)
        {
            var userId = GetUserId();

            var task = await _taskService.GetTask(projectId, taskId, userId);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [Authorize]
        [HttpPost("projects/{projectId}/tasks")]
        public async Task<ActionResult<TaskDto>> CreateTask(int projectId, [FromBody] CreateTaskDto createTaskDto)
        {
            var userId = GetUserId();

            try
            {
                var createdTask = await _taskService.CreateTask(projectId, createTaskDto, userId);

                return CreatedAtAction(
                    nameof(GetTask),
                    new { projectId, taskId = createdTask.Id },
                    createdTask);
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(ex.Message);
            }
        }
        [Authorize]
        [HttpPut("projects/{projectId}/tasks/{taskId}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(int projectId, int taskId, UpdateTaskDto taskDto)
        {
            var userId = GetUserId();

            try
            {
                var updatedTask = await _taskService.UpdateTask(projectId, taskId, taskDto, userId);
                return Ok(updatedTask);
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) when (ex.Message.Contains("does not belong"))
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "AdminsOnly")]
        [HttpDelete("projects/{projectId}/tasks/{taskId}")]
        public async Task<ActionResult> DeleteTask(int projectId, int taskId)
        {
            var userId = GetUserId();

            try
            {
                await _taskService.DeleteTask(projectId, taskId, userId);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) when (ex.Message.Contains("does not belong"))
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
