using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("api/perfilesprofesionales")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PerfilProfesionalController : ControllerBase
    {
        private readonly DataContext _context;

        public PerfilProfesionalController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.PerfilProfesionales.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var perfilprofesional = await _context.PerfilProfesionales.FirstOrDefaultAsync(x => x.Id == id);
            if (perfilprofesional == null)
            {
                return NotFound();
            }
            return Ok(perfilprofesional);
        }

        [HttpPost]
        public async Task<ActionResult> Post(PerfilProfesional perfilprofesional)
        {
            try
            {
                var usuario = await ObtenerUsuarioActualAsync();
                if (usuario == null) return Unauthorized("No se pudo identificar al usuario.");

                // Evita que un usuario cree perfiles duplicados
                var yaTienePerfil = await _context.PerfilProfesionales.AnyAsync(x => x.UserId == usuario.Id);
                if (yaTienePerfil && !User.IsInRole("Admin") && !User.IsInRole("Administrador"))
                {
                    return BadRequest("Ya tienes un perfil profesional creado.");
                }

                if (perfilprofesional.UserId == 0)
                {
                    perfilprofesional.UserId = usuario.Id;
                }

                _context.Add(perfilprofesional);
                await _context.SaveChangesAsync();
                return Ok(perfilprofesional);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(PerfilProfesional perfilprofesional)
        {
            try
            {
                var usuario = await ObtenerUsuarioActualAsync();
                if (usuario == null) return Unauthorized();

                var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
                if (perfilprofesional.UserId != usuario.Id && !esAdmin)
                {
                    return Forbid();
                }

                _context.Update(perfilprofesional);
                await _context.SaveChangesAsync();
                return Ok(perfilprofesional);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var perfilprofesional = await _context.PerfilProfesionales.FirstOrDefaultAsync(x => x.Id == id);
            if (perfilprofesional == null)
            {
                return NotFound();
            }

            var usuario = await ObtenerUsuarioActualAsync();
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (usuario == null || (perfilprofesional.UserId != usuario.Id && !esAdmin))
            {
                return Forbid();
            }

            _context.Remove(perfilprofesional);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("mi-perfil")]
        public async Task<ActionResult> GetMiPerfil()
        {
            // 1. Buscar cualquier claim que tenga un número entero (el UserId)
            var posiblesIds = new[]
            {
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            User.FindFirst("UserId")?.Value,
            User.FindFirst("id")?.Value,
            User.FindFirst("sub")?.Value
            };

            foreach (var idStr in posiblesIds)
            {
                if (int.TryParse(idStr, out int userId))
                {
                    var perfilPorId = await _context.PerfilProfesionales
                        .FirstOrDefaultAsync(x => x.UserId == userId);

                    if (perfilPorId != null)
                        return Ok(perfilPorId);
                }
            }

            // 2. Buscar cualquier claim que contenga el correo o UserName (ej. d38013787@gmail.com)
            var emailOUserName = User.Identity?.Name
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("email")?.Value
                ?? User.FindFirst("unique_name")?.Value
                ?? User.Claims.FirstOrDefault(c => c.Value.Contains("@"))?.Value;

            if (!string.IsNullOrEmpty(emailOUserName))
            {
                var usuario = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == emailOUserName || u.UserName == emailOUserName);

                if (usuario != null)
                {
                    var perfilPorUsuario = await _context.PerfilProfesionales
                        .FirstOrDefaultAsync(x => x.UserId == usuario.Id);

                    if (perfilPorUsuario != null)
                        return Ok(perfilPorUsuario);
                }
            }

            return NotFound();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(PerfilProfesional perfilprofesional)
        {
            _context.Update(perfilprofesional);
            await _context.SaveChangesAsync();
            return Ok(perfilprofesional);
        }

        [HttpGet("filter")]
        public async Task<ActionResult> GetFiltered(string text)
        {
            var query = _context.PerfilProfesionales
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.Trim().ToLower();
                query = query.Where(x =>
                    x.UserId.ToString().Contains(text) ||
                    x.NombreCompleto.ToLower().Contains(text) ||
                    x.TituloProfesional.ToLower().Contains(text));
            }

            var data = await query
                .OrderBy(x => x.UserId)
                .ThenBy(x => x.NombreCompleto)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("{id:int}/hateoas")]
        public async Task<ActionResult> GetWithLinks(int id)
        {
            var perfilprofesional = await _context.PerfilProfesionales
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (perfilprofesional == null)
            {
                return NotFound();
            }

            var response = new
            {
                perfilprofesional,
                links = new List<object>
                {
                    new { rel = "self", href = Url.Action("Get", new { id = perfilprofesional.Id }) },
                    new { rel = "update", href = Url.Action("Put") },
                    new { rel = "delete", href = Url.Action("Delete", new { id = perfilprofesional.Id }) }
                }
            };

            return Ok(response);
        }

        private async Task<User?> ObtenerUsuarioActualAsync()
        {
            var posiblesIds = new[]
            {
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
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
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Value.Contains("@"))?.Value;

            if (!string.IsNullOrEmpty(email))
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email || u.UserName == email);
            }

            return null;
        }
    }
}