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
    [Route("api/aplicaciones")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AplicacionController : ControllerBase
    {
        private readonly DataContext _context;

        public AplicacionController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (esAdmin)
            {
                return Ok(await _context.Aplicaciones.OrderByDescending(a => a.FechaPostulacion).ToListAsync());
            }

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return Unauthorized();

            var perfil = await _context.Set<PerfilProfesional>().FirstOrDefaultAsync(p => p.UserId == usuario.Id);
            if (perfil == null)
            {
                return Ok(new List<Aplicacion>());
            }

            var misAplicaciones = await _context.Aplicaciones
                .Where(a => a.PerfilProfesionalId == perfil.Id)
                .OrderByDescending(a => a.FechaPostulacion)
                .ToListAsync();

            return Ok(misAplicaciones);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var aplicacion = await _context.Aplicaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (aplicacion == null)
            {
                return NotFound();
            }

            if (!await PuedeGestionarAplicacionAsync(aplicacion))
            {
                return Forbid();
            }

            return Ok(aplicacion);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Aplicacion aplicacion)
        {
            try
            {
                var usuario = await ObtenerUsuarioActualAsync();
                if (usuario == null)
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");

                if (!esAdmin || aplicacion.PerfilProfesionalId <= 0)
                {
                    var perfil = await _context.Set<PerfilProfesional>().FirstOrDefaultAsync(p => p.UserId == usuario.Id);
                    if (perfil == null)
                    {
                        return BadRequest("Debes crear tu Perfil Profesional en la sección 'Mi Perfil' antes de postularte.");
                    }
                    aplicacion.PerfilProfesionalId = perfil.Id;
                }

                // Evitar postularse dos veces a la misma publicación con el mismo perfil
                var yaAplico = await _context.Aplicaciones.AnyAsync(a =>
                    a.PublicacionId == aplicacion.PublicacionId &&
                    a.PerfilProfesionalId == aplicacion.PerfilProfesionalId);

                if (yaAplico)
                {
                    return BadRequest("Ya te has postulado anteriormente a esta publicación.");
                }

                if (string.IsNullOrWhiteSpace(aplicacion.Estado))
                {
                    aplicacion.Estado = "Pendiente";
                }

                aplicacion.FechaPostulacion = DateTime.Now;

                _context.Add(aplicacion);
                await _context.SaveChangesAsync();
                return Ok(aplicacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Aplicacion aplicacion)
        {
            try
            {
                var existente = await _context.Aplicaciones.AsNoTracking().FirstOrDefaultAsync(x => x.Id == aplicacion.Id);
                if (existente == null) return NotFound();

                if (!await PuedeGestionarAplicacionAsync(existente))
                {
                    return Forbid();
                }

                var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");

                // Un usuario normal no puede alterar su propio estado ni cambiar el perfil/fecha original
                if (!esAdmin)
                {
                    aplicacion.Estado = existente.Estado;
                    aplicacion.PerfilProfesionalId = existente.PerfilProfesionalId;
                    aplicacion.FechaPostulacion = existente.FechaPostulacion;
                }

                _context.Update(aplicacion);
                await _context.SaveChangesAsync();
                return Ok(aplicacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var aplicacion = await _context.Aplicaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (aplicacion == null)
            {
                return NotFound();
            }

            if (!await PuedeGestionarAplicacionAsync(aplicacion))
            {
                return Forbid();
            }

            _context.Remove(aplicacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(Aplicacion aplicacion)
        {
            var existente = await _context.Aplicaciones.AsNoTracking().FirstOrDefaultAsync(x => x.Id == aplicacion.Id);
            if (existente == null) return NotFound();

            if (!await PuedeGestionarAplicacionAsync(existente))
            {
                return Forbid();
            }

            _context.Update(aplicacion);
            await _context.SaveChangesAsync();
            return Ok(aplicacion);
        }

        private async Task<bool> PuedeGestionarAplicacionAsync(Aplicacion aplicacion)
        {
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (esAdmin) return true;

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return false;

            var perfil = await _context.Set<PerfilProfesional>().FirstOrDefaultAsync(p => p.UserId == usuario.Id);
            return perfil != null && aplicacion.PerfilProfesionalId == perfil.Id;
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