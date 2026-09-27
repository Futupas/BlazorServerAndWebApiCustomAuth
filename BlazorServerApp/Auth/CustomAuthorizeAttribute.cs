namespace BlazorServerApp.Auth;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CustomAuthorizeAttribute : Attribute
{
    public string[] Roles { get; init; }

    public CustomAuthorizeAttribute() { }
    
    public CustomAuthorizeAttribute(string[] roles)
    {
        Roles = roles;
    }
}
