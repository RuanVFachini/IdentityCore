using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Domain;
using WebAPI.Domain.Models;
using WebAPI.Identity.DTO;
using WebAPI.Identity.DTO.Responses;

namespace WebAPI.Identity.Configuration
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserCreatedResponseDTO>().ReverseMap();
            CreateMap<User, UserLoginResponseDTO>().ReverseMap();

            CreateMap<Role, RoleCreatedResponseDTO>().ReverseMap();
        }
    }
}
