using BusinessLogic.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RakamonBackEnd.Middleware;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace RakamonBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }


        [HttpGet]
         public async Task<IActionResult> GetAllTasksAsync()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Token bulunamadı.");
            }

            var token = authHeader.Substring("Bearer ".Length);
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                var userId = jsonToken?.Claims.FirstOrDefault(claim =>
                    claim.Type == ClaimTypes.NameIdentifier)?.Value;

                var role = jsonToken?.Claims.FirstOrDefault(claim =>
                    claim.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                {
                    return Unauthorized("Geçersiz token bilgileri.");
                }

                if (role == "admin")
                {
                    var tasks = await _taskService.GetAllTasksAsync();
                    return Ok(tasks);
                }
                else
                {
                    var tasks = await _taskService.GetAllTasksAsync();
                    var filterTasks = tasks.Where(x => x.UserId.ToString() == userId).ToList();
                    return Ok(filterTasks);
                }
            }
            catch (Exception ex)
            {
                return Unauthorized("Token çözümlenirken hata oluştu: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskByIdAsync(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> AddTaskAsync([FromBody] RakamonTask task)
        {
            await _taskService.AddTaskAsync(task);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskAsync(int id, [FromBody] RakamonTask task)
        {
            task.Id = id;
            await _taskService.UpdateTaskAsync(task);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskAsync(int id)
        {
            await _taskService.DeleteTaskAsync(id);
            return Ok();
        }
    }
}
