using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("api/aplicaciones")]
    [AllowAnonymous]
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
            return Ok(await _context.Aplicaciones.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var aplicacion = await _context.Aplicaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (aplicacion == null)
            {
                return NotFound();
            }
            return Ok(aplicacion);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Aplicacion aplicacion)
        {
            try
            {
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
            _context.Remove(aplicacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(Aplicacion aplicacion)
        {
            _context.Update(aplicacion);
            await _context.SaveChangesAsync();
            return Ok(aplicacion);
        }
    }
}
