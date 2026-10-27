using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("/api/certificacion")]
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

        [HttpGet("{Id:int}")]
        public async Task<ActionResult> Get(int id)
        {

            var certificacion = await _context.Certificaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (certificacion == null)
            {
                return NotFound(); //404
            }
            return Ok(certificacion);


        }

        [HttpPost]
        public async Task<ActionResult> Post(Certificacion certificacion)
        {
            _context.Add(certificacion);
            await _context.SaveChangesAsync();
            return Ok(certificacion);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Certificacion certificacion)
        {
            _context.Update(certificacion);
            await _context.SaveChangesAsync();
            return Ok(certificacion);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var certificacion = await _context.Certificaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (certificacion == null)
            {
                return NotFound(); //404
            }
            _context.Remove(certificacion);
            await _context.SaveChangesAsync();
            return NoContent(); //204
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
