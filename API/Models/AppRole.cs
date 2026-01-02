using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRoleBridge>? Users { get; set; }
}
