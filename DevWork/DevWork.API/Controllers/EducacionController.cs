using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("api/educaciones")]
    [AllowAnonymous]
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
            return Ok(await _context.Educaciones.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var educacion = await _context.Educaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (educacion == null)
            {
                return NotFound();
            }
            return Ok(educacion);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Educacion educacion)
        {
            try
            {
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
            _context.Remove(educacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(Educacion educacion)
        {
            _context.Update(educacion);
            await _context.SaveChangesAsync();
            return Ok(educacion);
        }
    }
}