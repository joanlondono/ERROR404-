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
    [Route("api/experiencias")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExperienciaController : ControllerBase
    {
        private readonly DataContext _context;

        public ExperienciaController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (esAdmin)
            {
                return Ok(await _context.Experiencias
                    .OrderByDescending(e => e.FechaInicio)
                    .ToListAsync());
            }

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return Unauthorized();

            var perfil = await _context.Set<PerfilProfesional>()
                .FirstOrDefaultAsync(p => p.UserId == usuario.Id);

            if (perfil == null)
            {
                return Ok(new List<Experiencia>());
            }

            var misExperiencias = await _context.Experiencias
                .Where(e => e.PerfilProfesionalId == perfil.Id)
                .OrderByDescending(e => e.FechaInicio)
                .ToListAsync();

            return Ok(misExperiencias);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var experiencia = await _context.Experiencias.FirstOrDefaultAsync(x => x.Id == id);
            if (experiencia == null)
            {
                return NotFound();
            }

            if (!await PuedeGestionarExperienciaAsync(experiencia))
            {
                return Forbid();
            }

            return Ok(experiencia);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Experiencia experiencia)
        {
            try
            {
                var usuario = await ObtenerUsuarioActualAsync();
                if (usuario == null)
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");

                if (!esAdmin || experiencia.PerfilProfesionalId <= 0)
                {
                    var perfil = await _context.Set<PerfilProfesional>()
                        .FirstOrDefaultAsync(p => p.UserId == usuario.Id);

                    if (perfil == null)
                    {
                        return BadRequest("Debes crear tu Perfil Profesional en la sección 'Mi Perfil' antes de agregar experiencia laboral.");
                    }

                    experiencia.PerfilProfesionalId = perfil.Id;
                }

                if (experiencia.FechaFin.HasValue && experiencia.FechaFin.Value.Year > 1 && experiencia.FechaFin.Value < experiencia.FechaInicio)
                {
                    return BadRequest("La fecha de finalización no puede ser anterior a la fecha de inicio.");
                }

                _context.Add(experiencia);
                await _context.SaveChangesAsync();
                return Ok(experiencia);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Experiencia experiencia)
        {
            try
            {
                var existente = await _context.Experiencias.AsNoTracking().FirstOrDefaultAsync(x => x.Id == experiencia.Id);
                if (existente == null) return NotFound();

                if (!await PuedeGestionarExperienciaAsync(existente))
                {
                    return Forbid();
                }

                var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
                if (!esAdmin)
                {
                    experiencia.PerfilProfesionalId = existente.PerfilProfesionalId;
                }

                if (experiencia.FechaFin.HasValue && experiencia.FechaFin.Value.Year > 1 && experiencia.FechaFin.Value < experiencia.FechaInicio)
                {
                    return BadRequest("La fecha de finalización no puede ser anterior a la fecha de inicio.");
                }

                _context.Update(experiencia);
                await _context.SaveChangesAsync();
                return Ok(experiencia);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var experiencia = await _context.Experiencias.FirstOrDefaultAsync(x => x.Id == id);
            if (experiencia == null)
            {
                return NotFound();
            }

            if (!await PuedeGestionarExperienciaAsync(experiencia))
            {
                return Forbid();
            }

            _context.Remove(experiencia);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(Experiencia experiencia)
        {
            var existente = await _context.Experiencias.AsNoTracking().FirstOrDefaultAsync(x => x.Id == experiencia.Id);
            if (existente == null) return NotFound();

            if (!await PuedeGestionarExperienciaAsync(existente))
            {
                return Forbid();
            }

            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (!esAdmin)
            {
                experiencia.PerfilProfesionalId = existente.PerfilProfesionalId;
            }

            _context.Update(experiencia);
            await _context.SaveChangesAsync();
            return Ok(experiencia);
        }

        private async Task<bool> PuedeGestionarExperienciaAsync(Experiencia experiencia)
        {
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (esAdmin) return true;

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return false;

            var perfil = await _context.Set<PerfilProfesional>()
                .FirstOrDefaultAsync(p => p.UserId == usuario.Id);

            return perfil != null && experiencia.PerfilProfesionalId == perfil.Id;
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