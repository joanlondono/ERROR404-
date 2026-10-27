using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("/api/aplicacion")]
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

        [HttpGet("{Id:int}")]
        public async Task<ActionResult> Get(int id)
        {

            var aplicacion = await _context.Aplicaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (aplicacion == null)
            {
                return NotFound(); //404
            }
            return Ok(aplicacion);


        }

        [HttpPost]
        public async Task<ActionResult> Post(Aplicacion aplicacion)
        {
            _context.Add(aplicacion);
            await _context.SaveChangesAsync();
            return Ok(aplicacion);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Aplicacion aplicacion)
        {
            _context.Update(aplicacion);
            await _context.SaveChangesAsync();
            return Ok(aplicacion);
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
