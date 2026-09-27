using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace BlazorServerApp.Auth;

public class CustomAuthStateProvider
{
    public const string CookieName = "collectorshub_auth_cookie";
    public const int TokenExpirationTimeMin = 30;
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserService.UserService _userService;
    private readonly IConfiguration _configuration;

    public CustomAuthStateProvider(UserService.UserService userService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public AuthenticationState GetAuthenticationState()
    {
        try
        {
            if (!_httpContextAccessor.HttpContext!.Request.Cookies.ContainsKey(CookieName))
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }
            
            var token = _httpContextAccessor.HttpContext.Request.Cookies[CookieName];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var claims = jsonToken!.Claims.Select(claim => new Claim(claim.Type, claim.Value)).ToList();

            var claimsIdentity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(claimsIdentity);
            return new AuthenticationState(user);
        }
        catch (Exception)
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }
    
    /// <returns>Message if !ok, and token if ok</returns>
    public async Task<(bool ok, string message)> LoginAsync(string username, string password)
    {
        var user = await _userService.GetUserAsync(username, password);

        if (user is null) return (false, "Invalid username or password.");
        
        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? string.Empty),
        ]);

        // generate a JWT token based on the claims
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: Guid.NewGuid().ToString(),
            claims: claimsIdentity.Claims,
            expires: DateTime.Now.AddMinutes(TokenExpirationTimeMin),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())),
                SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return (true, tokenString);
    }
    
    public bool IsAuthenticated()
    {
        var state = GetAuthenticationState();
        return state.User.Identity?.IsAuthenticated == true;
    }

    public bool IsInRole(string[]? roles)
    {
        var state = GetAuthenticationState();

        if (!IsAuthenticated()) return false;

        return roles is null || roles.Length == 0 || roles.Any(state.User.IsInRole);
    }
}
