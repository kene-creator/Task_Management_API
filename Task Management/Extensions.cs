using Task_Management.Dtos;
using Task_Management.Dtos.UserDtos;
using Task_Management.Entities;
using Task_Management.Interface;
using Task_Management.Utility;
using Task = Task_Management.Entities.Task;

namespace Task_Management
{
    public static class Extensions
    {
        public static TaskDto AsTaskDto(this Task task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedAt = task.CreatedDate.DateTime,
                UpdatedAt = task.UpdatedDate?.DateTime,
                DueDate = task.DueDate,
                Priority = task.Priority
            };
        }

        public static UserDto AsUserDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }

        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

        public static void ConfigureIISIntegration(this IServiceCollection services) =>
            services.Configure<IISOptions>(options => { });

        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerManager, LoggerManager>();
    }
}