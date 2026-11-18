using DevWork.API.Helpers;
using DevWork.Shared.DTOs;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;

        public AccountsController(IUserHelper userHelper, IConfiguration configuration, IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _mailHelper = mailHelper;
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] UserDTO model)
        {
            try
            {
                var user = new User
                {
                    Email = model.Email,
                    UserName = model.Email,
                    RoleId = model.RoleId
                };

                // 1. Guardado de foto de perfil si el usuario seleccionó una
                if (!string.IsNullOrEmpty(model.Photo))
                {
                    var base64Data = model.Photo.Contains(",")
                        ? model.Photo.Split(',')[1]
                        : model.Photo;

                    var photoUser = Convert.FromBase64String(base64Data);
                    var fileName = $"{Guid.NewGuid()}.jpg";
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");
                    Directory.CreateDirectory(folderPath);
                    await System.IO.File.WriteAllBytesAsync(Path.Combine(folderPath, fileName), photoUser);
                    user.Photo = $"images/users/{fileName}";
                }

                // 2. Creación del usuario en Identity
                var result = await _userHelper.AddUserAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var roleName = await _userHelper.GetRoleNameAsync(user.RoleId);
                    if (string.IsNullOrWhiteSpace(roleName))
                    {
                        return BadRequest("El rol seleccionado no existe.");
                    }

                    await _userHelper.AddUserToRoleAsync(user, roleName);

                    // 3. Generación del enlace y envío del correo de confirmación
                    var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    var tokenLink = Url.Action("ConfirmEmail", "Accounts", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, HttpContext.Request.Scheme, HttpContext.Request.Host.Value);

                    var response = _mailHelper.SendMail(
                        user.Email!,
                        user.Email!,
                        "DevWork - Confirmación de cuenta",
                        $"<h1>DevWork - Confirmación de cuenta</h1>" +
                        $"<p>Para habilitar el usuario, por favor hacer clic en el siguiente enlace:</p>" +
                        $"<p><a href=\"{tokenLink}\">Confirmar Email</a></p>");

                    if (response.IsSuccess)
                    {
                        return NoContent();
                    }

                    return BadRequest(response.Message);
                }

                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error interno en CreateUser: {ex.Message} | {ex.InnerException?.Message}");
            }
        }

        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmailAsync(int userId, string token)
        {
            token = token.Replace(" ", "+");
            var user = await _userHelper.GetUserAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }

            return Redirect($"{_configuration["UrlFrontend"]}Login");
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

            if (result.IsLockedOut)
            {
                return BadRequest("Ha superado el máximo número de intentos, su cuenta está bloqueada, intente de nuevo en 5 minutos.");
            }

            if (result.IsNotAllowed)
            {
                return BadRequest("El usuario no ha sido habilitado, debes seguir las instrucciones del correo electrónico para poder habilitar el usuario.");
            }

            return BadRequest("Email o contraseña incorrectos.");
        }

        private async Task<TokenDTO> BuildTokenAsync(User user)
        {
            var roleName = await _userHelper.GetRoleNameAsync(user.RoleId);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
                new Claim("Photo", user.Photo ?? string.Empty)
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