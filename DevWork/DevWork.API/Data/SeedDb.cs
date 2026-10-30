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

        private async Task CheckUserAsync()
        {
            if (!_context.Users.Any())
            {
                var rolAdmin = _context.Roles.First(x => x.Nombre == "Administrador");
                var rolDev = _context.Roles.First(x => x.Nombre == "Desarrollador");

                _context.Users.Add(new User { Email = "admin@devwork.com", Password = "Admin123*", RoleId = rolAdmin.Id });
                _context.Users.Add(new User { Email = "diana@devwork.com", Password = "Dev123*", RoleId = rolDev.Id });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckPerfilProfesionalAsync()
        {
            if (!_context.PerfilProfesionales.Any())
            {
                var user = _context.Users.First(x => x.Email == "diana@devwork.com");
                _context.PerfilProfesionales.Add(new PerfilProfesional
                {
                    UserId = user.Id,
                    NombreCompleto = "Diana Desarrolladora",
                    TituloProfesional = "Ingeniera de Sistemas",
                    HabilidadesTecnicas = "C#, ASP.NET Core, SQL Server, Git",
                    AreaEspecialidad = "Desarrollo Backend",
                    TarifaHora = 45000,          
                    EnlaceGitHub = "https://github.com/diana-dev"
                });
                await _context.SaveChangesAsync();
            }
        }

    }
}

    
