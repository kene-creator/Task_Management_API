using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Management.Entities
{
    public record Task
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Title is required")]
        public required string Title { get; init; }

        [Required(ErrorMessage = "Description is required")]
        public required string Description { get; init; }

        [Required(ErrorMessage = "Created date is required")]
        public DateTimeOffset CreatedDate { get; init; }

        public DateTimeOffset? UpdatedDate { get; init; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; init; }

        [Range(1, int.MaxValue, ErrorMessage = "Priority must be a positive integer")]
        public int Priority { get; init; }

        [ForeignKey("User")]
        public required string UserId { get; init; }
        public User? User { get; init; }

    }
}
