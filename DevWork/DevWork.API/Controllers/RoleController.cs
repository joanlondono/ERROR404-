using DevWork.API.Data;
using DevWork.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Controllers
{
    [ApiController]
    [Route("/api/role")]
    public class RoleController : ControllerBase
    {
        private readonly DataContext _context;

        public RoleController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Roles.ToListAsync());
        }
        [HttpPost]
        public async Task<ActionResult> Post(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return Ok(role);
        }

    }
}
