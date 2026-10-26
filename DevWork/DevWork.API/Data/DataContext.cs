using DevWork.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace DevWork.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Publicacion> Publicaciones { get; set; }

        public DbSet<Aplicacion> Aplicaciones { get; set; }

        public DbSet<PerfilProfesional> PerfilProfesionales { get; set; }

        public DbSet<Educacion> Educaciones { get; set; }

        public DbSet<Experiencia> Experiencias { get; set; }

        public DbSet<Certificacion> Certificaciones { get; set; }

        public DbSet<Pregunta> Preguntas { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(c => c.Id).IsUnique();
        }


    }
}
