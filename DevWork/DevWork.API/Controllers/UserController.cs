using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult> Post(User users)
        {
            try
            {
                _context.Add(users);
                await _context.SaveChangesAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(User users)
        {
            try
            {
                var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == users.Id);
                if (currentUser == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                var roleExists = await _context.Roles.AnyAsync(r => r.Id == users.RoleId);
                if (!roleExists)
                {
                    return BadRequest($"El rol con Id {users.RoleId} no existe.");
                }

                currentUser.Email = users.Email;
                currentUser.UserName = users.Email;
                currentUser.NormalizedEmail = users.Email?.ToUpper();
                currentUser.NormalizedUserName = users.Email?.ToUpper();
                currentUser.RoleId = users.RoleId;

                if (!string.IsNullOrWhiteSpace(users.Password))
                {
                    currentUser.Password = users.Password;
                }

                await _context.SaveChangesAsync();
                return Ok(currentUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
    }
}
