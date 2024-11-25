using DataAccess.DB;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class TaskRepository : ITaskRepository
    {
       private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RakamonTask>> GetAllTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<RakamonTask> GetTaskByIdAsync(int id)
        {

            if(id == 0)
            {
                throw new ArgumentNullException("Id cannot be 0");
            }
            else
            {
                return await _context.Tasks.FindAsync(id);
            }
            
        }

        public async Task AddTaskAsync(RakamonTask task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(RakamonTask task)
        {
            try
            {
                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata oluştu: " + ex.Message);
                throw; // Hata detayını frontend'e yansıtmak için
            }
        }


        public async Task DeleteTaskAsync(int id)
        {
            var task = await GetTaskByIdAsync(id);
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }   
    }
}
