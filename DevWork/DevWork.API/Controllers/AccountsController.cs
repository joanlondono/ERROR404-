using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevWork.API.Helpers;
using DevWork.Shared.DTOs;
using DevWork.Shared.Models;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;

        public AccountsController(IUserHelper userHelper, IConfiguration configuration)
        {
            _userHelper = userHelper;
            _configuration = configuration;
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] UserDTO model)
        {
            User user = model;
            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                var roleName = await _userHelper.GetRoleNameAsync(user.RoleId);
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return BadRequest("El rol seleccionado no existe.");
                }

                await _userHelper.AddUserToRoleAsync(user, roleName);
                return Ok(await BuildTokenAsync(user));
            }

            return BadRequest(result.Errors.FirstOrDefault());
        }


        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO model)
        {
            var result = await _userHelper.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _userHelper.GetUserAsync(model.Email);
                if (user is null)
                {
                    return BadRequest("Email o contraseña incorrectos.");
                }

                return Ok(await BuildTokenAsync(user));
            }

            return BadRequest("Email o contraseña incorrectos.");
        }

        
        private async Task<TokenDTO> BuildTokenAsync(User user)
        {
            var roleName = await _userHelper.GetRoleNameAsync(user.RoleId);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
            };

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(30);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }

    }
}
