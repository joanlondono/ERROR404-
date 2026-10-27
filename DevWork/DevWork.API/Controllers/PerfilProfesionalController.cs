using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("/api/perfilprofesional")]
    public class PerfilProfesionalController:ControllerBase
    {
        private readonly DataContext _context;

        public PerfilProfesionalController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult> Get()
        {

            return Ok(await _context.PerfilProfesionales.ToListAsync());


        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult> Get(int id)
        {

            var perfilprofesional = await _context.PerfilProfesionales.FirstOrDefaultAsync(x => x.Id == id);
            if (perfilprofesional == null)
            {
                return NotFound(); //404
            }
            return Ok(perfilprofesional);


        }

        [HttpPost]
        public async Task<ActionResult> Post(PerfilProfesional perfilprofesional)
        {
            _context.Add(perfilprofesional);
            await _context.SaveChangesAsync();
            return Ok(perfilprofesional);
        }

        [HttpPut]
        public async Task<ActionResult> Put(PerfilProfesional perfilprofesional)
        {
            _context.Update(perfilprofesional);
            await _context.SaveChangesAsync();
            return Ok(perfilprofesional);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var perfilprofesional = await _context.PerfilProfesionales.FirstOrDefaultAsync(x => x.Id == id);
            if (perfilprofesional == null)
            {
                return NotFound(); //404
            }
            _context.Remove(perfilprofesional);
            await _context.SaveChangesAsync();
            return NoContent(); //204
        }


        [HttpPatch]

        public async Task<ActionResult> Patch(PerfilProfesional perfilprofesional)
        {
            _context.Update(perfilprofesional);
            await _context.SaveChangesAsync();
            return Ok(perfilprofesional);

        }

    }
}
