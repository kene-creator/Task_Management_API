using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task_Management.Dtos;
using Task_Management.Interface;
using Task = Task_Management.Entities.Task;

namespace Catalog_API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly ITaskRepository _taskRepository;

        public TaskController(ITaskRepository taskRepository, ILoggerManager logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> GetTaskById(string id)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(id);
                if (task == null)
                {
                    _logger.LogInfo($"Task with id: {id}, hasn't been found in database.");
                    return NotFound();
                }

                _logger.LogInfo($"Returned task with id: {id}");
                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetTaskById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Task>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var task = new Task
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    DueDate = createTaskDto.DueDate,
                    Priority = createTaskDto.Priority,
                    UserId = userId
                };

                await _taskRepository.AddTaskAsync(task);
                _logger.LogInfo($"Task with id: {task.Id} created successfully.");
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateTask action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] Task task)
        {
            try
            {
                if (id != task.Id)
                {
                    _logger.LogError($"Task ID mismatch: id ({id}) != task.Id ({task.Id})");
                    return BadRequest("Task ID mismatch");
                }

                await _taskRepository.UpdateTaskAsync(task);
                _logger.LogInfo($"Task with id: {id} updated successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateTask action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(id);
                if (task == null)
                {
                    _logger.LogError($"Task with id: {id}, hasn't been found in database.");
                    return NotFound();
                }

                _taskRepository.DeleteTaskAsync(id);
                _logger.LogInfo($"Task with id: {id} deleted successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteTask action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
