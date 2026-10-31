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
            await CheckUserAsync();
            await CheckPerfilProfesionalAsync();
        }
        private async Task CheckRoleAsync()
        {
            if (!_context.Roles.Any())
            {
                _context.Roles.Add(new Role { Name = "Desarrollador" });
                _context.Roles.Add(new Role { Name = "Empresa" });
                _context.Roles.Add(new Role { Name = "Administrador" });
                _context.Roles.Add(new Role { Name = "Cliente" });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckUserAsync()
        {
            if (!_context.Users.Any())
            {
                var rolAdmin = _context.Roles.First(x => x.Name == "Administrador");
                var rolDev = _context.Roles.First(x => x.Name == "Desarrollador");

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
                    Biografia = "Desarrolladora de software apasionada por el desarrollo Backend con C# y SQL Server.",
                    HabilidadesTecnicas = "C#, ASP.NET Core, SQL Server, Git",
                    AreaEspecialidad = "Desarrollo Backend",
                    TarifaHora = 45000m,          
                    EnlaceGitHub = "https://github.com/diana-dev",
                    EnlaceLinkedIn = "https://linkedin.com/in/diana-dev"
                });
                await _context.SaveChangesAsync();
            }
        }

    }
}

    
