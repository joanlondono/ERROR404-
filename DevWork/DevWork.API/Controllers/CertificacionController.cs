using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("api/certificaciones")]
    [AllowAnonymous]
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
            return Ok(await _context.Certificaciones.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var certificacion = await _context.Certificaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (certificacion == null)
            {
                return NotFound();
            }
            return Ok(certificacion);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Certificacion certificacion)
        {
            try
            {
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
            _context.Remove(certificacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult> Patch(Certificacion certificacion)
        {
            _context.Update(certificacion);
            await _context.SaveChangesAsync();
            return Ok(certificacion);
        }
    }
}