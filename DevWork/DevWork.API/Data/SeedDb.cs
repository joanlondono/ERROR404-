using DevWork.API.Helpers;
using DevWork.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace DevWork.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckUserAsync("admin@devwork.com", "Admin123*", "Administrador");
            await CheckUserAsync("diana@devwork.com", "Dev123*", "Desarrollador");
            await CheckPerfilProfesionalAsync();
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync("Desarrollador");
            await _userHelper.CheckRoleAsync("Empresa");
            await _userHelper.CheckRoleAsync("Administrador");
            await _userHelper.CheckRoleAsync("Cliente");
        }

        private async Task<User> CheckUserAsync(string email, string password, string roleName)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                var rol = await _context.Roles.FirstAsync(x => x.Name == roleName);

                user = new User
                {
                    Email = email,
                    UserName = email,
                    RoleId = rol.Id,
                    EmailConfirmed = true
                };

                await _userHelper.AddUserAsync(user, password);
                await _userHelper.AddUserToRoleAsync(user, roleName);
            }

            return user;
        }

        private async Task CheckPerfilProfesionalAsync()
        {
            if (!_context.PerfilProfesionales.Any())
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == "diana@devwork.com");
                if (user != null)
                {
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
}
