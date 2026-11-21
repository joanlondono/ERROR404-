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
    [Route("api/educaciones")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EducacionController : ControllerBase
    {
        private readonly DataContext _context;

        public EducacionController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (esAdmin)
            {
                return Ok(await _context.Educaciones
                    .OrderByDescending(e => e.FechaInicio)
                    .ToListAsync());
            }

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return Unauthorized();

            var perfil = await _context.Set<PerfilProfesional>()
                .FirstOrDefaultAsync(p => p.UserId == usuario.Id);

            if (perfil == null)
            {
                return Ok(new List<Educacion>());
            }

            var miEducacion = await _context.Educaciones
                .Where(e => e.PerfilProfesionalId == perfil.Id)
                .OrderByDescending(e => e.FechaInicio)
                .ToListAsync();

            return Ok(miEducacion);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var educacion = await _context.Educaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (educacion == null)
            {
                return NotFound();
            }

            if (!await PuedeGestionarEducacionAsync(educacion))
            {
                return Forbid();
            }

            return Ok(educacion);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Educacion educacion)
        {
            try
            {
                var usuario = await ObtenerUsuarioActualAsync();
                if (usuario == null)
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");

                if (!esAdmin || educacion.PerfilProfesionalId <= 0)
                {
                    var perfil = await _context.Set<PerfilProfesional>()
                        .FirstOrDefaultAsync(p => p.UserId == usuario.Id);

                    if (perfil == null)
                    {
                        return BadRequest("Debes crear tu Perfil Profesional en la sección 'Mi Perfil' antes de agregar formación académica.");
                    }

                    educacion.PerfilProfesionalId = perfil.Id;
                }

                if (educacion.FechaFin.HasValue && educacion.FechaFin.Value.Year > 1 && educacion.FechaFin.Value < educacion.FechaInicio)
                {
                    return BadRequest("La fecha de finalización no puede ser anterior a la fecha de inicio.");
                }

                _context.Add(educacion);
                await _context.SaveChangesAsync();
                return Ok(educacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Educacion educacion)
        {
            try
            {
                var existente = await _context.Educaciones.AsNoTracking().FirstOrDefaultAsync(x => x.Id == educacion.Id);
                if (existente == null) return NotFound();

                if (!await PuedeGestionarEducacionAsync(existente))
                {
                    return Forbid();
                }

                var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
                if (!esAdmin)
                {
                    educacion.PerfilProfesionalId = existente.PerfilProfesionalId;
                }

                if (educacion.FechaFin.HasValue && educacion.FechaFin.Value.Year > 1 && educacion.FechaFin.Value < educacion.FechaInicio)
                {
                    return BadRequest("La fecha de finalización no puede ser anterior a la fecha de inicio.");
                }

                _context.Update(educacion);
                await _context.SaveChangesAsync();
                return Ok(educacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var educacion = await _context.Educaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (educacion == null)
            {
                return NotFound();
            }

            if (!await PuedeGestionarEducacionAsync(educacion))
            {
                return Forbid();
            }

            _context.Remove(educacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(Educacion educacion)
        {
            var existente = await _context.Educaciones.AsNoTracking().FirstOrDefaultAsync(x => x.Id == educacion.Id);
            if (existente == null) return NotFound();

            if (!await PuedeGestionarEducacionAsync(existente))
            {
                return Forbid();
            }

            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (!esAdmin)
            {
                educacion.PerfilProfesionalId = existente.PerfilProfesionalId;
            }

            _context.Update(educacion);
            await _context.SaveChangesAsync();
            return Ok(educacion);
        }

        private async Task<bool> PuedeGestionarEducacionAsync(Educacion educacion)
        {
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (esAdmin) return true;

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return false;

            var perfil = await _context.Set<PerfilProfesional>()
                .FirstOrDefaultAsync(p => p.UserId == usuario.Id);

            return perfil != null && educacion.PerfilProfesionalId == perfil.Id;
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