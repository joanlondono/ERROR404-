using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("api/roles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RoleController : ControllerBase
    {
        private readonly DataContext _context;

        public RoleController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Roles.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Role role)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(role.Name))
                {
                    role.NormalizedName = role.Name.Trim().ToUpper();
                }

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Role role)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(role.Name))
                {
                    role.NormalizedName = role.Name.Trim().ToUpper();
                }

                _context.Roles.Update(role);
                await _context.SaveChangesAsync();
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
            if (role == null)
            {
                return NotFound();
            }
            _context.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
