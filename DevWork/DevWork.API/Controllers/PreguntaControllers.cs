using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("/api/pregunta")]
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

        [HttpGet("{Id:int}")]
        public async Task<ActionResult> Get(int id)
        {

            var pregunta = await _context.Preguntas.FirstOrDefaultAsync(x => x.Id == id);
            if (pregunta == null)
            {
                return NotFound(); //404
            }
            return Ok(pregunta);


        }

        [HttpPost]
        public async Task<ActionResult> Post(Pregunta pregunta)
        {
            _context.Add(pregunta);
            await _context.SaveChangesAsync();
            return Ok(pregunta);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Pregunta pregunta)
        {
            _context.Update(pregunta);
            await _context.SaveChangesAsync();
            return Ok(pregunta);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var pregunta = await _context.Preguntas.FirstOrDefaultAsync(x => x.Id == id);
            if (pregunta == null)
            {
                return NotFound(); //404
            }
            _context.Remove(pregunta);
            await _context.SaveChangesAsync();
            return NoContent(); //204
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
