using DevWork.Shared.Models;
using System.Diagnostics.Eventing.Reader;

namespace DevWork.API.Data
{
    public class SeedDb
    {

        private readonly DataContext _context;
        public SeedDb(DataContext context)
        {
            _context = context;
        }
        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRoleAsync();
        }
        private async Task CheckRoleAsync()
        {
            if (!_context.Roles.Any())
            {
                _context.Roles.Add(new Role { Nombre = "Desarrollador" });
                _context.Roles.Add(new Role { Nombre = "Empresa" });
                _context.Roles.Add(new Role { Nombre = "Administrador" });
                _context.Roles.Add(new Role { Nombre = "Cliente" });
                await _context.SaveChangesAsync();
            }
        }
    }
}

    
