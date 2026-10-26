using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevWork.API.Data;
using DevWork.API.Models;

namespace DevWork.API.Controllers
{

    [ApiController]
    [Route("/api/Publicaciones")]
    public class PublicacionController : ControllerBase
    {
        private readonly DataContext _context;
        public PublicacionController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetPublicacion(string? area, string? tipo)
        {
            var query = _context.Publicaciones.AsQueryable();
            if (!string.IsNullOrEmpty(area))
            {
                query = query.Where(p => p.AreaSistemas == area);
            }
            if (!string.IsNullOrEmpty(tipo))
            {
                query = query.Where(p => p.TipoOferta == tipo);
            }
            var resultado = await query.ToListAsync();
            return Ok(resultado);
        }
        [HttpPost]
        public async Task<IActionResult> PostPublicacion(Publicacion publicacion)
        {
            if (publicacion == null)
            {
                return BadRequest();
            }

            _context.Publicacion.Add(publicacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPublicacionPorId), new { id = publicacion.Id }, publicacion);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPublicacionPorId(int id)
        {
            var publicacion = await _context.Publicacion.FindAsync(id);
            if (publicacion == null)
            {
                return NotFound();
            }
            return Ok(publicacion);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublicacion(int id)
        {
            var publicacion = await _context.Publicacion.FindAsync(id);
            if (publicacion == null)
            {
                return NotFound();
            }

            _context.Publicacion.Remove(publicacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("{publicacionId}/aplicar")]
        public async Task<IActionResult> AplicarAPublicacion(int publicacionId, Aplicacion aplicacion)
        {
            var publicacion = await _context.Publicacion.FindAsync(publicacionId);
            if (publicacion == null)
            {
                return NotFound();
            }

            // Lógica para aplicar a la publicación
            aplicacion.PublicacionId = publicacionId;
            aplicacion.FechaPostulacion = DateTime.Now;
            _context.Aplicaciones.Add(aplicacion);
            await _context.SaveChangesAsync();

            return Ok(publicacion);
        }

        [HttpPost("{publicacionId}/preguntas")]
        public async Task<IActionResult> PostPregunta(int publicacionId, Pregunta pregunta)
        {
            var publicacion = await _context.Publicacion.FindAsync(publicacionId);
            if (publicacion == null)
            {
                return NotFound("La publicación no existe.");
            }

            pregunta.PublicacionId = publicacionId;
            _context.Preguntas.Add(pregunta);
            await _context.SaveChangesAsync();

            return Ok(pregunta);
        }

        [HttpGet("{publicacionId}/preguntas")]
        public async Task<IActionResult> GetPreguntasPorPublicacion(int publicacionId)
        {
            var preguntas = await _context.Preguntas
                .Where(p => p.PublicacionId == publicacionId)
                .ToListAsync();

            return Ok(preguntas);
        }

    }

}
