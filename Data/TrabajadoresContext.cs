using Microsoft.EntityFrameworkCore;
using TrabajadoresApp.Models;

namespace TrabajadoresApp.Data
{
    public class TrabajadoresContext : DbContext
    {
        public TrabajadoresContext(DbContextOptions<TrabajadoresContext> options) : base(options)
        {
        }

        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Distrito> Distritos { get; set; }
        public DbSet<Trabajador> Trabajadores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relaciones
            modelBuilder.Entity<Provincia>()
                .HasOne(p => p.Departamento)
                .WithMany(d => d.Provincias)
                .HasForeignKey(p => p.IdDepartamento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Distrito>()
                .HasOne(d => d.Provincia)
                .WithMany(p => p.Distritos)
                .HasForeignKey(d => d.IdProvincia)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trabajador>()
                .HasOne(t => t.Departamento)
                .WithMany(d => d.Trabajadores)
                .HasForeignKey(t => t.IdDepartamento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trabajador>()
                .HasOne(t => t.Provincia)
                .WithMany(p => p.Trabajadores)
                .HasForeignKey(t => t.IdProvincia)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trabajador>()
                .HasOne(t => t.Distrito)
                .WithMany(d => d.Trabajadores)
                .HasForeignKey(t => t.IdDistrito)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}