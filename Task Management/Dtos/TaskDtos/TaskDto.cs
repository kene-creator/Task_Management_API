using System;

namespace Task_Management.Dtos
{
    public record TaskDto
    {
        public required string Id { get; init; }
        public required string Title { get; init; }
        public required string Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public DateTime DueDate { get; init; }
        public int Priority { get; init; }
    }
}
