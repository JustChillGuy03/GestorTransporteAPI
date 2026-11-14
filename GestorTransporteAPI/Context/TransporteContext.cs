using GestorTransporteAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorTransporteAPI.Context
{
    public class TransporteContext : DbContext
    {
        public TransporteContext(DbContextOptions<TransporteContext> options) : base(options) { }

        public DbSet<Chofer> Choferes { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Chofer>(entity =>
            {
                entity.ToTable("Choferes");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Licencia).IsRequired().HasMaxLength(20);
                entity.Property(c => c.Telefono).HasMaxLength(20);

                // No puede haber dos choferes con la misma licencia
                entity.HasIndex(c => c.Licencia).IsUnique();
            });

            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.ToTable("Vehiculos");
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Marca).IsRequired().HasMaxLength(50);
                entity.Property(v => v.Modelo).IsRequired().HasMaxLength(50);
                entity.Property(v => v.Placa).IsRequired().HasMaxLength(10);

                // No puede haber dos vehículos con la misma placa
                entity.HasIndex(v => v.Placa).IsUnique();

                // Un chofer tiene muchos vehículos; no se permite borrar un chofer
                // que todavía tenga vehículos asignados (Restrict en vez de Cascade)
                entity.HasOne(v => v.Chofer)
                      .WithMany(c => c.Vehiculos)
                      .HasForeignKey(v => v.ChoferId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Datos iniciales para probar la API
            modelBuilder.Entity<Chofer>().HasData(
                new Chofer { Id = 1, Nombre = "Juan Pérez", Licencia = "LIC-00001", Telefono = "809-555-0101" },
                new Chofer { Id = 2, Nombre = "María Rodríguez", Licencia = "LIC-00002", Telefono = "829-555-0202" }
            );

            modelBuilder.Entity<Vehiculo>().HasData(
                new Vehiculo { Id = 1, Marca = "Toyota", Modelo = "Hiace", Placa = "A100001", Capacidad = 15, ChoferId = 1 },
                new Vehiculo { Id = 2, Marca = "Hyundai", Modelo = "County", Placa = "A100002", Capacidad = 30, ChoferId = 2 }
            );
        }
    }
}
