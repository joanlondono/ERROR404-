using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("api/preguntas")]
    [AllowAnonymous]
    public class PreguntaController : ControllerBase
    {
        private readonly DataContext _context;

        public PreguntaController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Preguntas.ToListAsync());
        }

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
            _context.Remove(pregunta);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(Pregunta pregunta)
        {
            _context.Update(pregunta);
            await _context.SaveChangesAsync();
            return Ok(pregunta);
        }
    }
}

