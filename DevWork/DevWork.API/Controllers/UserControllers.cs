using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{

    [ApiController]
    [Route("/api/user")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
        }

        //Get con lista
        //Select * From owners

        [HttpGet]
        public async Task<ActionResult> Get()
        {

            return Ok(await _context.Users.ToListAsync());


        }

        // Get por parámetro
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {

            //200 Ok

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {


                return NotFound();
            }

            return Ok(user);


        }


        [HttpPost]
        public async Task<ActionResult> Post(User users)
        {
            _context.Add(users);
            await _context.SaveChangesAsync();
            return Ok(users);

        }


        [HttpPut]
        public async Task<ActionResult> Put(User users)
        {
            _context.Add(users);
            await _context.SaveChangesAsync();
            return Ok(users);

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {


                return NotFound();
            }
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch]

        public async Task<ActionResult> Patch(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
            return Ok(user);

        }

    }
}