using Task = Task_Management.Entities.Task;

namespace Task_Management.Interface
{
    public interface ITaskRepository
    {
        Task<IEnumerable<Task>> GetTasksByUserIdAsync(string userId);
        Task<Task> GetTaskByIdAsync(string taskId);
        Task<Task> AddTaskAsync(Task task);
        Task<Task> UpdateTaskAsync(Task task);
        void DeleteTaskAsync(string taskId);
    }
}
