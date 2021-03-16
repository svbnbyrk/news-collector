using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using NewsCollector.DTO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NewsCollector.Helpers;
using AutoMapper;

namespace NewsCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;
        private readonly IMapper mapper;

        public UserController(IUserService userService, IOptions<AppSettings> appSettings, IMapper mapper)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
            this.mapper = mapper;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate(AuthenticateRequest model)
        {

            var response = await AuthenticateAsync(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPost("")]
        public async Task<ActionResult<AddUserDTO>> CreateUser([FromBody] AddUserDTO addUserResources)
        {
            User userModel = mapper.Map<AddUserDTO, User>(addUserResources);
            var newUser = await _userService.CreateUser(userModel);
            if(newUser == null)
                return NotFound();
            var user = await _userService.GetUserById(newUser.Id);
            var userModelDTO = mapper.Map<User, AddUserDTO>(user);
            return Ok(userModelDTO);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetUserById(id);
            if(user == null)
                return NotFound();
                
            await _userService.DeleteUser(user);
            return Ok();
        }
        
        [NonAction]
        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        [NonAction]
        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model)
        {
            var user = await _userService.GetUserByUsernamePassword(model.Username, model.Password);

            // return null if user not found
            if (user == null) 
                return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

    }
}
