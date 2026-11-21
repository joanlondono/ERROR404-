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
    [Route("api/certificaciones")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CertificacionController : ControllerBase
    {
        private readonly DataContext _context;

        public CertificacionController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (esAdmin)
            {
                return Ok(await _context.Certificaciones
                    .OrderByDescending(c => c.FechaObtencion)
                    .ToListAsync());
            }

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return Unauthorized();

            var perfil = await _context.Set<PerfilProfesional>()
                .FirstOrDefaultAsync(p => p.UserId == usuario.Id);

            if (perfil == null)
            {
                return Ok(new List<Certificacion>());
            }

            var misCertificaciones = await _context.Certificaciones
                .Where(c => c.PerfilProfesionalId == perfil.Id)
                .OrderByDescending(c => c.FechaObtencion)
                .ToListAsync();

            return Ok(misCertificaciones);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var certificacion = await _context.Certificaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (certificacion == null)
            {
                return NotFound();
            }

            if (!await PuedeGestionarCertificacionAsync(certificacion))
            {
                return Forbid();
            }

            return Ok(certificacion);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Certificacion certificacion)
        {
            try
            {
                var usuario = await ObtenerUsuarioActualAsync();
                if (usuario == null)
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");

                if (!esAdmin || certificacion.PerfilProfesionalId <= 0)
                {
                    var perfil = await _context.Set<PerfilProfesional>()
                        .FirstOrDefaultAsync(p => p.UserId == usuario.Id);

                    if (perfil == null)
                    {
                        return BadRequest("Debes crear tu Perfil Profesional en la sección 'Mi Perfil' antes de agregar certificaciones.");
                    }

                    certificacion.PerfilProfesionalId = perfil.Id;
                }

                _context.Add(certificacion);
                await _context.SaveChangesAsync();
                return Ok(certificacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Certificacion certificacion)
        {
            try
            {
                var existente = await _context.Certificaciones.AsNoTracking().FirstOrDefaultAsync(x => x.Id == certificacion.Id);
                if (existente == null) return NotFound();

                if (!await PuedeGestionarCertificacionAsync(existente))
                {
                    return Forbid();
                }

                var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
                if (!esAdmin)
                {
                    certificacion.PerfilProfesionalId = existente.PerfilProfesionalId;
                }

                _context.Update(certificacion);
                await _context.SaveChangesAsync();
                return Ok(certificacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var certificacion = await _context.Certificaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (certificacion == null)
            {
                return NotFound();
            }

            if (!await PuedeGestionarCertificacionAsync(certificacion))
            {
                return Forbid();
            }

            _context.Remove(certificacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(Certificacion certificacion)
        {
            var existente = await _context.Certificaciones.AsNoTracking().FirstOrDefaultAsync(x => x.Id == certificacion.Id);
            if (existente == null) return NotFound();

            if (!await PuedeGestionarCertificacionAsync(existente))
            {
                return Forbid();
            }

            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (!esAdmin)
            {
                certificacion.PerfilProfesionalId = existente.PerfilProfesionalId;
            }

            _context.Update(certificacion);
            await _context.SaveChangesAsync();
            return Ok(certificacion);
        }

        private async Task<bool> PuedeGestionarCertificacionAsync(Certificacion certificacion)
        {
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (esAdmin) return true;

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return false;

            var perfil = await _context.Set<PerfilProfesional>()
                .FirstOrDefaultAsync(p => p.UserId == usuario.Id);

            return perfil != null && certificacion.PerfilProfesionalId == perfil.Id;
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