using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("/api/publicacion")]
    public class PublicacionController : ControllerBase
    {
        private readonly DataContext _context;

        public PublicacionController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {

            return Ok(await _context.Publicaciones.ToListAsync());


        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult> Get(int id)
        {

            var publicacion = await _context.Publicaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (publicacion == null)
            {
                return NotFound(); //404
            }
            return Ok(publicacion);


        }

        [HttpPost]
        public async Task<ActionResult> Post(Publicacion publicacion)
        {
            _context.Add(publicacion);
            await _context.SaveChangesAsync();
            return Ok(publicacion);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Publicacion publicacion)
        {
            _context.Update(publicacion);
            await _context.SaveChangesAsync();
            return Ok(publicacion);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var publicacion  = await _context.Publicaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (publicacion == null)
            {
                return NotFound(); //404
            }
            _context.Remove(publicacion);
            await _context.SaveChangesAsync();
            return NoContent(); //204
        }


        [HttpPatch]

        public async Task<ActionResult> Patch(Publicacion publicacion)
        {
            _context.Update(publicacion);
            await _context.SaveChangesAsync();
            return Ok(publicacion);

        }

    }
}
