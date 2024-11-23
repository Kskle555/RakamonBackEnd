using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
namespace DataAccess.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<RakamonTask>> GetAllTasksAsync();
        Task<RakamonTask> GetTaskByIdAsync(int id);
        Task AddTaskAsync(RakamonTask task);
        Task UpdateTaskAsync(RakamonTask task);
        Task DeleteTaskAsync(int id);
    }
}
