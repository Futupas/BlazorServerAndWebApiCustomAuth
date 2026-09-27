namespace UserService.Models;

/// <summary>
/// This is just an example. IRL you will have different model.
/// </summary>
public record User
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string Name { get; init; }
    /// <summary> This should be PasswordHash, but for simplicity I will leave like this </summary>
    public string Password { get; init; }
    public Role? Role { get; init; }
}
