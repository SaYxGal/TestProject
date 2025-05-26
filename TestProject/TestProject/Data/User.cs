using Microsoft.AspNetCore.Identity;

namespace TestProject.Data;

public class User : IdentityUser<int>
{
    public string FullName { get; set; } = null!;
}
