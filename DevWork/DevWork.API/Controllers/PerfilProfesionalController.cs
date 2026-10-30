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

        [HttpGet("filter")]

        public async Task<ActionResult> GetFiltered(string text)
        {
            var query = _context.PerfilProfesionales

                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.Trim().ToLower();
                query = query.Where( x =>  
                    x.UserId.ToString().Contains(text) ||
                    x.NombreCompleto.ToLower().Contains(text) ||
                    x.TituloProfesional.ToLower().Contains(text));




            }
            var data = await query
                .OrderBy(x => x.UserId)
                .ThenBy(x => x.NombreCompleto)
                .ToListAsync();
            return Ok(data);

        }

        [HttpGet("{id}:int/hateoas")]
        public async Task<ActionResult> GetWithLinks(int id)
        {

            var perfilprofesional = await _context.PerfilProfesionales
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (perfilprofesional == null)
            {
                return NotFound(); //404
            }
            var response = new
            {
                perfilprofesional,
                links = new List<object>
                {
                    new { rel = "self", href = Url.Action("Get", new { id = perfilprofesional.Id }) },
                    new { rel = "update", href = Url.Action("Put") },
                    new { rel = "delete", href = Url.Action("Delete", new { id = perfilprofesional.Id }) }
                }
            };
            return Ok(response);
        }

    }
}
