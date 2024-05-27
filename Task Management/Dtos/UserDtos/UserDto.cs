using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_Management.Enums;

namespace Task_Management.Dtos.UserDtos
{
    public record UserDto
    {
        public required string Id { get; init; }
        public required string Email { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }

    }
}