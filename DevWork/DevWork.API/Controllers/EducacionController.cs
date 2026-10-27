using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("/api/educacion")]
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

        [HttpGet("{Id:int}")]
        public async Task<ActionResult> Get(int id)
        {

            var educacion = await _context.Educaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (educacion == null)
            {
                return NotFound(); //404
            }
            return Ok(educacion);


        }

        [HttpPost]
        public async Task<ActionResult> Post(Educacion educacion)
        {
            _context.Add(educacion);
            await _context.SaveChangesAsync();
            return Ok(educacion);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Educacion educacion)
        {
            _context.Update(educacion);
            await _context.SaveChangesAsync();
            return Ok(educacion);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var educacion = await _context.Educaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (educacion == null)
            {
                return NotFound(); //404
            }
            _context.Remove(educacion);
            await _context.SaveChangesAsync();
            return NoContent(); //204
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
