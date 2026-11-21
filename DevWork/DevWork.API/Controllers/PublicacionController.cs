using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("api/publicaciones")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PublicacionController : ControllerBase
    {
        private readonly DataContext _context;

        public PublicacionController(DataContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Publicaciones.ToListAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var publicacion = await _context.Publicaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (publicacion == null)
            {
                return NotFound();
            }
            return Ok(publicacion);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Publicacion publicacion)
        {
            try
            {
                var usuario = await ObtenerUsuarioActualAsync();
                if (usuario == null)
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                // Si tu modelo Publicacion tiene UserId, lo asigna automáticamente
                if (publicacion.UserId == 0)
                {
                    publicacion.UserId = usuario.Id;
                }

                _context.Add(publicacion);
                await _context.SaveChangesAsync();
                return Ok(publicacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Publicacion publicacion)
        {
            try
            {
                var usuario = await ObtenerUsuarioActualAsync();
                if (usuario == null) return Unauthorized();

                var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
                if (publicacion.UserId != usuario.Id && !esAdmin)
                {
                    return Forbid();
                }

                _context.Update(publicacion);
                await _context.SaveChangesAsync();
                return Ok(publicacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var publicacion = await _context.Publicaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (publicacion == null)
            {
                return NotFound();
            }

            var usuario = await ObtenerUsuarioActualAsync();
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (usuario == null || (publicacion.UserId != usuario.Id && !esAdmin))
            {
                return Forbid();
            }

            _context.Remove(publicacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(Publicacion publicacion)
        {
            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return Unauthorized();

            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (publicacion.UserId != usuario.Id && !esAdmin)
            {
                return Forbid();
            }

            _context.Update(publicacion);
            await _context.SaveChangesAsync();
            return Ok(publicacion);
        }

        private async Task<User?> ObtenerUsuarioActualAsync()
        {
            var posiblesIds = new[]
            {
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                User.FindFirst("UserId")?.Value,
                User.FindFirst("id")?.Value,
                User.FindFirst("sub")?.Value
            };

            foreach (var idStr in posiblesIds)
            {
                if (int.TryParse(idStr, out int userId))
                {
                    var u = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                    if (u != null) return u;
                }
            }

            var email = User.Identity?.Name
                ?? User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Value.Contains("@"))?.Value;

            if (!string.IsNullOrEmpty(email))
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email || u.UserName == email);
            }

            return null;
        }
    }
}