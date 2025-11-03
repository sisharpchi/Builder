namespace Contract.Dto.Auth;

public class CreateUserDto
{
    public string FirstName { get; set; } = default!;
    public string? LastName { get; set; }
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Phonenumber { get; set; } = default!;
}
