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

        // 1. Postulaciones que YO he enviado a ofertas de otros
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (esAdmin)
            {
                return Ok(await _context.Aplicaciones
                    .Include(a => a.PerfilProfesional)
                    .OrderByDescending(a => a.FechaPostulacion)
                    .ToListAsync());
            }

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return Unauthorized();

            var perfil = await _context.Set<PerfilProfesional>().FirstOrDefaultAsync(p => p.UserId == usuario.Id);
            if (perfil == null)
            {
                return Ok(new List<Aplicacion>());
            }

            var misAplicaciones = await _context.Aplicaciones
                .Include(a => a.PerfilProfesional)
                .Where(a => a.PerfilProfesionalId == perfil.Id)
                .OrderByDescending(a => a.FechaPostulacion)
                .ToListAsync();

            return Ok(misAplicaciones);
        }

        // 2. Nuevo: Postulaciones RECIBIDAS en las publicaciones creadas por mí
        [HttpGet("recibidas")]
        public async Task<ActionResult> GetRecibidas()
        {
            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return Unauthorized();

            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");

            var query = _context.Aplicaciones
                .Include(a => a.PerfilProfesional)
                .AsQueryable();

            if (!esAdmin)
            {
                var idsMisPublicaciones = await _context.Set<Publicacion>()
                    .Where(p => p.UserId == usuario.Id)
                    .Select(p => p.Id)
                    .ToListAsync();

                query = query.Where(a => idsMisPublicaciones.Contains(a.PublicacionId));
            }

            var recibidas = await query
                .OrderByDescending(a => a.FechaPostulacion)
                .ToListAsync();

            return Ok(recibidas);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var aplicacion = await _context.Aplicaciones
                .Include(a => a.PerfilProfesional)
                .FirstOrDefaultAsync(x => x.Id == id);

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

                // Evitar postularse a su propia publicación
                var publicacion = await _context.Set<Publicacion>().FirstOrDefaultAsync(p => p.Id == aplicacion.PublicacionId);
                if (publicacion == null)
                {
                    return NotFound("La publicación no existe.");
                }

                if (!esAdmin && publicacion.UserId == usuario.Id)
                {
                    return BadRequest("No puedes postularte a tu propia publicación.");
                }

                if (!esAdmin || aplicacion.PerfilProfesionalId <= 0)
                {
                    var perfil = await _context.Set<PerfilProfesional>().FirstOrDefaultAsync(p => p.UserId == usuario.Id);
                    if (perfil == null)
                    {
                        return BadRequest("Debes crear tu Perfil Profesional en la sección 'Mi Perfil' antes de postularte.");
                    }
                    aplicacion.PerfilProfesionalId = perfil.Id;
                }

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
                var esDuenoPublicacion = await EsDuenoDePublicacionAsync(existente.PublicacionId);

                if (!esAdmin)
                {
                    // Siempre se protegen los datos inmutables
                    aplicacion.PerfilProfesionalId = existente.PerfilProfesionalId;
                    aplicacion.PublicacionId = existente.PublicacionId;
                    aplicacion.FechaPostulacion = existente.FechaPostulacion;

                    if (esDuenoPublicacion)
                    {
                        // El dueño de la oferta SOLO puede cambiar el Estado (Aceptada / Rechazada), no el mensaje del candidato
                        aplicacion.PropuestaMensaje = existente.PropuestaMensaje;
                    }
                    else
                    {
                        // El candidato puede editar su mensaje mientras esté pendiente, pero NO cambiar su propio Estado
                        aplicacion.Estado = existente.Estado;
                    }
                }

                // Evitar problemas de tracking de EF Core con propiedades de navegación
                aplicacion.PerfilProfesional = null;
                aplicacion.Publicacion = null;

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

        private async Task<bool> PuedeGestionarAplicacionAsync(Aplicacion aplicacion)
        {
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");
            if (esAdmin) return true;

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return false;

            // 1. Es el candidato que envió la postulación
            var perfil = await _context.Set<PerfilProfesional>().FirstOrDefaultAsync(p => p.UserId == usuario.Id);
            if (perfil != null && aplicacion.PerfilProfesionalId == perfil.Id)
            {
                return true;
            }

            // 2. O es el dueño de la publicación a la que postularon
            return await EsDuenoDePublicacionAsync(aplicacion.PublicacionId);
        }

        private async Task<bool> EsDuenoDePublicacionAsync(int publicacionId)
        {
            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return false;

            return await _context.Set<Publicacion>()
                .AnyAsync(p => p.Id == publicacionId && p.UserId == usuario.Id);
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