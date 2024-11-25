using BusinessLogic.Services;
using DataAccess.DB;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RakamonBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly AppDbContext _context;
  

        public TasksController(ITaskService taskService, AppDbContext context)
        {
            _taskService = taskService;
            _context = context;
        }

        private (string? userId, string? role) GetUserInfoFromToken()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return (null, null);
            }

            try
            {
                var token = authHeader.Substring("Bearer ".Length);
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                var userId = jsonToken?.Claims.FirstOrDefault(claim =>
                    claim.Type == ClaimTypes.NameIdentifier)?.Value;
                var role = jsonToken?.Claims.FirstOrDefault(claim =>
                    claim.Type == ClaimTypes.Role)?.Value;

                return (userId, role);
            }
            catch
            {
                return (null, null);
            }
        }

        private async Task<bool> IsUserAuthorizedForTask(int taskId, string userId, string role)
        {
            if (role == "admin") return true;

            var task = await _taskService.GetTaskByIdAsync(taskId);
            return task != null && task.UserId.ToString() == userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasksAsync()
        {
            try
            {

            
            var (userId, role) = GetUserInfoFromToken();
            if (userId == null || role == null)
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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskByIdAsync(int id)
        {
            try
            {

            
            var (userId, role) = GetUserInfoFromToken();
            if (userId == null || role == null)
            {
                return Unauthorized("Geçersiz token bilgileri.");
            }

            if (!await IsUserAuthorizedForTask(id, userId, role))
            {
                return Forbid("Bu göreve erişim yetkiniz bulunmamaktadır.");
            }

            var task = await _taskService.GetTaskByIdAsync(id);
            return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTaskAsync([FromBody] RakamonTask task)
        {
            try
            {

            
            var (userId, role) = GetUserInfoFromToken();
                // Eğer TcKimlikNo gönderildiyse, UserId'yi çözümle
            var user = await _context.Users.SingleOrDefaultAsync(u => u.TcKimlikNo.ToString() == userId.ToString());
             if (user == null)
              {
                 return BadRequest($"Belirtilen TcKimlikNo ({task.UserId}) ile eşleşen bir kullanıcı bulunamadı.");
              }
                if (userId == null || role == null)
            {
                return Unauthorized("Geçersiz token bilgileri.");
            }

            // Eğer admin değilse, sadece kendi userId'si ile task oluşturabilir
            if (role != "admin" && task.UserId.ToString() != userId)
            {
                return Forbid("Başka bir kullanıcı için görev oluşturamazsınız.");
            }

            //var userid = user.Id;
            //task.UserId = user.Id;
            task.User = null;
            await _taskService.AddTaskAsync(task);
            return Ok();
            }
             catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskAsync(int id, [FromBody] RakamonTask task)
        {
            try
            {

           
            var (userId, role) = GetUserInfoFromToken();
            if (userId == null || role == null)
            {
                return Unauthorized("Geçersiz token bilgileri.");
            }

            if (!await IsUserAuthorizedForTask(id, userId, role))
            {
                return Forbid("Bu görevi düzenleme yetkiniz bulunmamaktadır.");
            }


            // Eğer TcKimlikNo gönderildiyse, UserId'yi çözümle
            var user = await _context.Users.SingleOrDefaultAsync(u => u.TcKimlikNo.ToString() == task.UserId.ToString());
            if (user == null)
            {
               return BadRequest($"Belirtilen TcKimlikNo ({task.UserId}) ile eşleşen bir kullanıcı bulunamadı.");
            }

            // Admin değilse, görevin UserId'sini değiştirememeli
            if (role != "admin" && task.UserId.ToString() != userId)
            {
                return Forbid("Görevi başka bir kullanıcıya atayamazsınız.");
            }

            task.UserId = user.Id; 
            task.Id = id;
            task.User = null; // Gereksiz navigasyon verilerini temizle
            await _taskService.UpdateTaskAsync(task);
          
            return Ok();
            }
                        catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskAsync(int id)
        {
            try
            {


                var (userId, role) = GetUserInfoFromToken();
                if (userId == null || role == null)
                {
                    return Unauthorized("Geçersiz token bilgileri.");
                }

                if (!await IsUserAuthorizedForTask(id, userId, role))
                {
                    return Forbid("Bu görevi silme yetkiniz bulunmamaktadır.");
                }

                await _taskService.DeleteTaskAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}