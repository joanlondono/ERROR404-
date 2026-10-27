using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("/api/experiencia")]
    public class ExperienciaController : ControllerBase
    {
        private readonly DataContext _context;

        public ExperienciaController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {

            return Ok(await _context.Experiencias.ToListAsync());


        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult> Get(int id)
        {

            var experiencia = await _context.Experiencias.FirstOrDefaultAsync(x => x.Id == id);
            if (experiencia == null)
            {
                return NotFound(); //404
            }
            return Ok(experiencia);


        }

        [HttpPost]
        public async Task<ActionResult> Post(Experiencia experiencia)
        {
            _context.Add(experiencia);
            await _context.SaveChangesAsync();
            return Ok(experiencia);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Experiencia experiencia)
        {
            _context.Update(experiencia);
            await _context.SaveChangesAsync();
            return Ok(experiencia);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var experiencia = await _context.Experiencias.FirstOrDefaultAsync(x => x.Id == id);
            if (experiencia == null)
            {
                return NotFound(); //404
            }
            _context.Remove(experiencia);
            await _context.SaveChangesAsync();
            return NoContent(); //204
        }


        [HttpPatch]

        public async Task<ActionResult> Patch(Experiencia experiencia)
        {
            _context.Update(experiencia);
            await _context.SaveChangesAsync();
            return Ok(experiencia);

        }

    }
}
