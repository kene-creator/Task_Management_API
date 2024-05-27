using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_Management.Data;
using Task_Management.Entities;
using Task_Management.Interface;
using Task = Task_Management.Entities.Task;

namespace Task_Management.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskManagementDbContext _context;

        public TaskRepository(TaskManagementDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Task>> FindAsync(System.Linq.Expressions.Expression<Func<Task, bool>> predicate)
        {
            return await _context.Tasks.Where(predicate).ToListAsync();
        }

        public async Task<Task> GetByIdAsync(string id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<Task> AddAsync(Task entity)
        {
            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }



        public async Task<IEnumerable<Task>> GetTasksByUserIdAsync(string userId)
        {
            return await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task<Task> GetTaskByIdAsync(string taskId)
        {
            return await _context.Tasks.FindAsync(taskId);
        }

        public async Task<Task> AddTaskAsync(Task task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<Task> UpdateTaskAsync(Task task)
        {
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return task;
        }

        public async void DeleteTaskAsync(string taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

    }
}
