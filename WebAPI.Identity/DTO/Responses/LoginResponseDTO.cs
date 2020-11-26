using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Identity.DTO.Responses
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public UserLoginResponseDTO User { get; set; }
    }
}
