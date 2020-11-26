using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WebAPI.Domain.Models
{
    public class Role : IdentityRole<int>
    {
        public IEnumerable<UserRole> UserRoles { get; set; }
    }
}
