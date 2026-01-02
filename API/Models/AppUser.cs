using System;
using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class AppUser : IdentityUser<int>
{
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public ICollection<AppUserRoleBridge>? Roles { get; set; }

}
