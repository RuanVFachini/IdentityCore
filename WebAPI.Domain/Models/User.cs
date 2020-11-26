using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using WebAPI.Domain.Models;

namespace WebAPI.Domain
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public string Member { get; set; } = "Member";
        public int? OrgId { get; set; }

        public IEnumerable<UserRole> UserRoles { get; set; }
    }
}
