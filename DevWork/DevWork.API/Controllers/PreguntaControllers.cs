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
    [Route("api/preguntas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PreguntaController : ControllerBase
    {
        private readonly DataContext _context;

        public PreguntaController(DataContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Preguntas.ToListAsync());
        }

        [AllowAnonymous]
        [HttpGet("por-publicacion/{publicacionId:int}")]
        public async Task<ActionResult> GetPorPublicacion(int publicacionId)
        {
            var comentarios = await (from p in _context.Preguntas
                                     join u in _context.Users on p.UserId equals u.Id into userJoin
                                     from usuario in userJoin.DefaultIfEmpty()
                                     where p.PublicacionId == publicacionId
                                     orderby p.Id descending
                                     select new
                                     {
                                         p.Id,
                                         p.PublicacionId,
                                         p.TextoPregunta,
                                         p.UserId,
                                         AutorNombre = usuario != null ? (usuario.UserName ?? usuario.Email) : "Usuario",
                                         AutorEmail = usuario != null ? usuario.Email : string.Empty
                                     }).ToListAsync();

            return Ok(comentarios);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var pregunta = await _context.Preguntas.FirstOrDefaultAsync(x => x.Id == id);
            if (pregunta == null)
            {
                return NotFound();
            }
            return Ok(pregunta);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Pregunta pregunta)
        {
            try
            {
                var usuario = await ObtenerUsuarioActualAsync();
                if (usuario == null)
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                pregunta.UserId = usuario.Id;

                _context.Add(pregunta);
                await _context.SaveChangesAsync();
                return Ok(pregunta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Pregunta pregunta)
        {
            try
            {
                var existente = await _context.Preguntas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pregunta.Id);
                if (existente == null) return NotFound();

                var usuario = await ObtenerUsuarioActualAsync();
                var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");

                if (usuario == null || (existente.UserId != usuario.Id && !esAdmin))
                {
                    return Forbid();
                }

                pregunta.UserId = existente.UserId; // Mantiene al dueño original
                _context.Update(pregunta);
                await _context.SaveChangesAsync();
                return Ok(pregunta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var pregunta = await _context.Preguntas.FirstOrDefaultAsync(x => x.Id == id);
            if (pregunta == null)
            {
                return NotFound();
            }

            var usuario = await ObtenerUsuarioActualAsync();
            var esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");

            if (usuario == null || (pregunta.UserId != usuario.Id && !esAdmin))
            {
                return Forbid();
            }

            _context.Remove(pregunta);
            await _context.SaveChangesAsync();
            return NoContent();
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