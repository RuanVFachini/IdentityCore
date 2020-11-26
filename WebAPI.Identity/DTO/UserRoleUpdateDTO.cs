using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Domain.Models;

namespace WebAPI.Identity.DTO
{
    public class UserRoleUpdateDTO
    {
        public string Email { get; set; }
        public bool Delete { get; set; }
        public string Role { get; set; }
    }
}
