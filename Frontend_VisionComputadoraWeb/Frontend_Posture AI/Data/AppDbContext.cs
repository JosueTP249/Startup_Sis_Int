using Frontedn_Posture_AI.Models.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Frontedn_Posture_AI.Data
{
    /// <summary>
    /// Contexto de Entity Framework Core para la base de datos relacional
    /// (PostgreSQL) que sostiene el modelo SaaS B2B de Posture AI.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Clinica> Clinicas => Set<Clinica>();
        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<Ejercicio> Ejercicios => Set<Ejercicio>();
        public DbSet<EvaluacionPostural> EvaluacionesPosturales => Set<EvaluacionPostural>();

        // Ids fijos de los datos demo, usados cuando el frontend no envía un paciente
        public const int ClinicaDemoId = 1;
        public const int PacienteDemoId = 1;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- Clinicas ----------
            modelBuilder.Entity<Clinica>(e =>
            {
                e.ToTable("Clinicas");
                e.HasIndex(c => c.Nombre).IsUnique();
            });

            // ---------- Pacientes ----------
            modelBuilder.Entity<Paciente>(e =>
            {
                e.ToTable("Pacientes");
                e.HasOne(p => p.Clinica)
                 .WithMany(c => c.Pacientes)
                 .HasForeignKey(p => p.ClinicaId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(p => p.ClinicaId);
            });

            // ---------- Ejercicios (catálogo de clases del modelo) ----------
            modelBuilder.Entity<Ejercicio>(e =>
            {
                e.ToTable("Ejercicios");
                e.Property(x => x.Id).ValueGeneratedNever();
            });

            // ---------- EvaluacionesPosturales (historial) ----------
            modelBuilder.Entity<EvaluacionPostural>(e =>
            {
                e.ToTable("EvaluacionesPosturales");
                e.HasOne(ev => ev.Paciente)
                 .WithMany(p => p.Evaluaciones)
                 .HasForeignKey(ev => ev.UsuarioId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(ev => ev.Ejercicio)
                 .WithMany(x => x.Evaluaciones)
                 .HasForeignKey(ev => ev.EjercicioId)
                 .OnDelete(DeleteBehavior.SetNull);

                // Consultas de historial: por paciente ordenado por fecha, y por estado
                e.HasIndex(ev => new { ev.UsuarioId, ev.FechaHora });
                e.HasIndex(ev => ev.Estado);
            });

            SembrarDatos(modelBuilder);
        }

        private static void SembrarDatos(ModelBuilder modelBuilder)
        {
            var fechaSemilla = new DateTime(2026, 11, 19, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Clinica>().HasData(new Clinica
            {
                Id = ClinicaDemoId,
                Nombre = "Clínica Demo Posture AI",
                Plan = "Profesional",
                Ciudad = "Demo",
                FechaAlta = fechaSemilla,
                Activa = true
            });

            modelBuilder.Entity<Paciente>().HasData(new Paciente
            {
                Id = PacienteDemoId,
                ClinicaId = ClinicaDemoId,
                Nombre = "Paciente Demo",
                Empresa = "Jurado Pitch",
                FechaRegistro = fechaSemilla
            });

            // Mismo orden que la salida del modelo MLP (índices 0 a 12)
            modelBuilder.Entity<Ejercicio>().HasData(
                new Ejercicio { Id = 0, Nombre = "Puente de glúteo", Fase = "Abajo" },
                new Ejercicio { Id = 1, Nombre = "Puente de glúteo", Fase = "Arriba" },
                new Ejercicio { Id = 2, Nombre = "Elevación de rodillas", Fase = "Abajo" },
                new Ejercicio { Id = 3, Nombre = "Elevación de rodillas", Fase = "Arriba" },
                new Ejercicio { Id = 4, Nombre = "Elevación lateral de pierna", Fase = "Abajo" },
                new Ejercicio { Id = 5, Nombre = "Elevación lateral de pierna", Fase = "Arriba" },
                new Ejercicio { Id = 6, Nombre = "Elevación lateral de brazos", Fase = "Abajo" },
                new Ejercicio { Id = 7, Nombre = "Elevación lateral de brazos", Fase = "Arriba" },
                new Ejercicio { Id = 8, Nombre = "Elevación de piernas", Fase = "Abajo" },
                new Ejercicio { Id = 9, Nombre = "Elevación de piernas", Fase = "Arriba" },
                new Ejercicio { Id = 10, Nombre = "Flexión de hombro", Fase = "Arriba" },
                new Ejercicio { Id = 11, Nombre = "Press de hombro", Fase = "Abajo" },
                new Ejercicio { Id = 12, Nombre = "Press de hombro", Fase = "Arriba" }
            );
        }
    }
}
