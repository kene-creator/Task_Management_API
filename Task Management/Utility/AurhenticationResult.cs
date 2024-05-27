using Task_Management.Entities;

public class AuthenticationResult
{
    public required User User { get; set; }
    public required string ErrorMessage { get; set; }
}