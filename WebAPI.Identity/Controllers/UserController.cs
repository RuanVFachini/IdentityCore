using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Domain;
using WebAPI.Identity.DTO;
using WebAPI.Identity.DTO.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public UserController(IConfiguration config, UserManager<User> userManager,
                              SignInManager<User> signInManager, IMapper mapper)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        // POST api/<UserController>
        [HttpPost("login")]
        [AllowAnonymous]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] UserLoginDTO model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                if (result.Succeeded)
                {
                    var appUser = await _userManager.Users.FirstOrDefaultAsync(u =>
                            u.NormalizedUserName == user.UserName.ToUpper());

                    var userDTO = _mapper.Map<UserLoginResponseDTO>(appUser);

                    return base.Ok(new LoginResponseDTO()
                    {
                        Token = GenerateJwt(appUser).Result,
                        User = userDTO
                    });
                }

                return Unauthorized();
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR {e.Message}");
            }
        }

        // POST api/<UserController>
        [HttpPost()]
        [AllowAnonymous]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]    
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserCreatedResponseDTO>> Post([FromBody] UserDTO model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    user = new User
                    {
                        FullName = model.FullName,
                        UserName = model.UserName,
                        Email = model.Email
                    };

                    var result = await _userManager.CreateAsync(
                        user, model.Password);

                    if (result.Succeeded)
                    {
                        //var appUser = await _userManager.Users.FirstOrDefaultAsync(u => 
                        //    u.NormalizedUserName == user.UserName.ToUpper());

                        //var token = GenerateJwt(appUser).Result;
                        //var confirmationEmail = Url.Action("ConfirmEmailAddress", "Home",
                        //    new { token = token, email = user.Email }, Request.Scheme);

                        //System.IO.File.WriteAllText("confirmationEmail.txt", confirmationEmail);

                        return StatusCode(201, _mapper.Map<UserCreatedResponseDTO>(user));
                    }
                }

                return Unauthorized();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"ERROR {e.Message}");
            }
        }

        private async Task<string> GenerateJwt(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach(var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var param = _config.GetSection("AppSettings:Token:SecurityKey").Value;

            var key = new SymmetricSecurityKey(
                                Encoding.ASCII.GetBytes(param));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }
    }
}
