using UserService.Models;

namespace UserService;

/// <summary>
/// This will actually something like <see cref="DbSet" /> oe a real user service. For simplicity, I will leave like this
/// </summary>
public class UserService
{
    public IList<Role> Roles { get; private init; }
    public IList<User> Users { get; private init; }

    
    public UserService()
    {
        // Create roles
        var roleUser = new Role()
        {
            Id = 1,
            Name = "User",
        };
        var roleAdmin = new Role()
        {
            Id = 2,
            Name = "Admin",
        };
        Roles = new List<Role>()
        {
            roleUser,
            roleAdmin,
        };
        
        // Create users
        Users = new List<User>()
        {
            new User
            {
                Id = 11,
                Username = "dude",
                Name = "Jeffrey Lebowski",
                Password = "the_dude",
                Role = roleAdmin,
            },
            new User
            {
                Id = 12,
                Username = "vincent",
                Name = "Vincent Vega",
                Password = "v_vega",
                Role = roleUser
            },
            new User
            {
                Id = 13,
                Username = "borat",
                Name = "Borat Sagdiyev",
                Password = "great_success",
                Role = null,
            }
        };
    }

    /// <summary> <see cref="Task"/> is needed to simulate real <see cref="DbSet"/> </summary>
    public Task<User?> GetUserAsync(string username, string password)
    {
        var user = Users.FirstOrDefault(x => x.Username == username && x.Password == password);

        return Task.FromResult(user);
    }
}
