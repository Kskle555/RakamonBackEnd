using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BusinessLogic.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<RakamonTask>> GetAllTasksAsync();
        Task<RakamonTask> GetTaskByIdAsync(int id);
        Task AddTaskAsync(RakamonTask task);
        Task UpdateTaskAsync(RakamonTask task);
        Task DeleteTaskAsync(int id);   
    }
}
