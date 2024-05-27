using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Task_Management.Enums;

namespace Task_Management.Entities
{
    public record User
    {
        [Key]
        [Column("UserId")]
        public required string Id { get; init; }

        [Required(ErrorMessage = "CreatedAt is required.")]
        public DateTimeOffset CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; init; }

        [Required(ErrorMessage = "PasswordHash is required.")]
        public required string PasswordHash { get; set; }

        public string? RefreshToken { get; set; }

        public string? EmailToken { get; init; }

        public string? ResetToken { get; init; }

        public DateTime? ResetTokenExpiresAt { get; init; }

        public bool? EmailValid { get; init; }

        [Required(ErrorMessage = "FirstName is required.")]
        public required string FirstName { get; init; }

        [Required(ErrorMessage = "LastName is required.")]
        public required string LastName { get; init; }

        public int? FailedSignInAttempts { get; init; }

        public ICollection<Task>? Tasks { get; init; }
    }
}
