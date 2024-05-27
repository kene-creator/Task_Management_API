using System;
using System.ComponentModel.DataAnnotations;

namespace Task_Management.Dtos
{
    public record UpdateTaskDto
    {
        [Required]
        public required string Title { get; init; }

        [Required]
        public required string Description { get; init; }

        [Required]
        public required DateTime DueDate { get; init; }

        [Required]
        [Range(1, 5, ErrorMessage = "Priority must be between 1 and 5.")]
        public int Priority { get; init; }
    }
}
