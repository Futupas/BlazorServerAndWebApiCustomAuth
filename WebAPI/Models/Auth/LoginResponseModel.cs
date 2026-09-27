namespace WebAPI.Models.Auth;

public class LoginResponseModel
{
    public string Token { get; init; }
    public string Username { get; init; }
    public string? Role { get; init; }
}
